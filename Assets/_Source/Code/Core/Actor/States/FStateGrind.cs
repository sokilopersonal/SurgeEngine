using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.Actor.System.Characters.Sonic;
using SurgeEngine.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Code.Gameplay.CommonObjects.Mobility.Rails;
using SurgeEngine.Code.Infrastructure.Custom;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateGrind : FStateMove, IBoostHandler
    {
        private Rail _rail;
        private SplineSample _sample;
        private Vector3 _prevTg;
        private bool _isForward;
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
                _rigidbody.position += _sample.up * 0.05f;
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
                Vector3 pos = _rigidbody.position - Actor.transform.up * _rail.Radius;
                Vector3 localPos = _rail.transform.InverseTransformPoint(pos);
                SplineUtility.GetNearestPoint(spline, localPos, out var p, out var f);

                _sample = new SplineSample
                {
                    pos = _rail.Container.EvaluatePosition(f),
                    tg = ((Vector3)spline.EvaluateTangent(f)).normalized,
                    up = spline.EvaluateUpVector(f)
                };
                
                if (_prevTg == Vector3.zero) _prevTg = _sample.tg;
                
                Vector3 upSplinePlane = Vector3.Cross(_sample.tg, _sample.up);
                
                Kinematics.Project(_sample.up);
                Kinematics.Project(upSplinePlane);
                
                float sign = Mathf.Sign(Vector3.Dot(_rigidbody.transform.forward, _sample.tg));
                _isForward = Mathf.Approximately(sign, 1);
                Vector3 direction = _isForward ? _sample.tg : -_sample.tg;
                _rigidbody.rotation = Quaternion.LookRotation(direction, _sample.up);
                
                _prevTg = _sample.tg;

                Vector3 newPos = _sample.pos;
                newPos += _sample.up * (1 + _rail.Radius);

                Vector3 planeNormal = Vector3.ProjectOnPlane(_sample.tg, _sample.up);
                SurgeMath.SplitPlanarVector(_rigidbody.position, 
                    planeNormal, 
                    out _, 
                    out var pVer);
                
                SurgeMath.SplitPlanarVector(newPos, planeNormal, 
                    out var sLat,
                    out _);
                
                Vector3 slopeForce = Vector3.ProjectOnPlane(Vector3.down, _sample.up) * _grindGravityPower;
                _rigidbody.AddForce(slopeForce * Time.fixedDeltaTime, ForceMode.Impulse);
                
                _rigidbody.position = sLat + pVer;
                
                Kinematics.Normal = _sample.up;

                if (!_rail.Container.Spline.Closed)
                {
                    if (IsRailCooldown()) return;

                    const float THRESHOLD = 0.0005f;
                    bool outOfTime = 1 - f < THRESHOLD || f < THRESHOLD;
                    
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
            _rail = rail;
        }
        
        public bool IsRailCooldown() => _timer > 0;

        public void BoostHandle()
        {
            Actor.stateMachine.GetSubState<FBoost>().BaseGroundBoost();
        }
    }
}