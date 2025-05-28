using System.Diagnostics;
using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Mobility.Rails;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;
using UnityEngine.Splines;
using Debug = UnityEngine.Debug;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateGrind : FStateMove, IBoostHandler
    {
        private Rail _rail;
        private SplineSample _sample;
        private bool _isForward;

        private float _railTime;
        private float _railLength;
        private float _timer;

        protected float _grindGravityPower;
        
        public FStateGrind(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _grindGravityPower = 10f;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0.25f;
            _rigidbody.linearVelocity = Vector3.ClampMagnitude(_rigidbody.linearVelocity, Actor.config.topSpeed);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (Input.JumpPressed)
            {
                StateMachine.SetState<FStateGrindJump>();
                _rigidbody.position += _sample.up * 0.25f;
            }

            if (this is not FStateGrindSquat)
            {
                if (Input.BHeld)
                {
                    StateMachine.SetState<FStateGrindSquat>().SetRail(_rail);
                }
            }

            if (_timer > 0) _timer -= dt;
            else _timer = 0;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (_rail != null)
            {
                var spline = _rail.Container.Spline;
                float normalizedTime = _railTime / _railLength;
                spline.Evaluate(normalizedTime, out var pos, out var tg, out var up);
                tg = _rail.transform.TransformDirection(tg);
                
                _sample = new SplineSample
                {
                    pos = pos,
                    tg = ((Vector3)tg).normalized,
                    up = up
                };
                
                Vector3 right = Vector3.Cross(_sample.tg, Vector3.up).normalized;
                Vector3 targetUp = _rail.transform.TransformDirection(up);
                
                Debug.DrawRay(_rail.transform.TransformPoint(pos), targetUp, Color.red, 3f);
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, targetUp);
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, right);
                
                Vector3 endPos = pos + up * (1 + _rail.Radius);
                _rigidbody.position = _rail.transform.TransformPoint(endPos);
                
                Vector3 targetTangent = _isForward ? _sample.tg : -_sample.tg;
                _rigidbody.rotation = Quaternion.LookRotation(targetTangent, targetUp);
                
                _railTime += Vector3.Dot(_rigidbody.linearVelocity, _sample.tg) * dt;

                if (!_rail.Container.Spline.Closed)
                {
                    if (IsRailCooldown()) return;

                    const float THRESHOLD = 0.0005f;
                    bool outOfTime = 1 - normalizedTime < THRESHOLD || normalizedTime < THRESHOLD;
                    
                    if (outOfTime)
                    {
                        _rigidbody.position += _rigidbody.transform.forward * 0.3f;
                        StateMachine.SetState<FStateAir>();
                    }
                }
            }
        }

        public void SetRail(Rail rail)
        {
            Vector3 pos = _rigidbody.position - Actor.transform.up * rail.Radius;
            var spline = rail.Container.Spline;
            SplineUtility.GetNearestPoint(spline, rail.transform.InverseTransformPoint(pos), out var near, out var f);
            
            _railLength = spline.GetLength();
            _railTime = f * _railLength;

            float dot = Vector3.Dot(_rigidbody.transform.forward,
                rail.transform.TransformDirection(spline.EvaluateTangent(_railTime)));
            _isForward = dot > 0;
            
            _rail = rail;
        }
        
        public bool IsRailCooldown() => _timer > 0;

        public void BoostHandle()
        {
            Actor.stateMachine.GetSubState<FBoost>().BaseGroundBoost();
        }
    }
}