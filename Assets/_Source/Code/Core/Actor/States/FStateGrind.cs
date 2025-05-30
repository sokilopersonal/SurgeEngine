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
        private SplineData _data;
        
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
            
            _rigidbody.linearVelocity = Vector3.ClampMagnitude(_rigidbody.linearVelocity, Actor.Config.topSpeed);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (Input.JumpPressed)
            {
                SetCooldown(0.1f);
                StateMachine.SetState<FStateGrindJump>();
            }

            if (this is not FStateGrindSquat)
            {
                if (Input.BHeld)
                {
                    StateMachine.SetState<FStateGrindSquat>()?.Share(_rail, _data, _isForward);
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
                _data.EvaluateWorld(out var pos,  out var tg, out var targetUp, out var right);

                Debug.DrawRay(pos, tg, Color.white);
                Debug.DrawRay(pos, targetUp, Color.green);
                Debug.DrawRay(pos, right, Color.yellow);
                
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, targetUp);
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, right);
                
                Vector3 downForce = Vector3.ProjectOnPlane(Vector3.down, targetUp) * _grindGravityPower;
                _rigidbody.AddForce(downForce * dt, ForceMode.Impulse);
                
                Vector3 endPos = pos + targetUp * (1 + _rail.Radius);
                _rigidbody.position = endPos;
                
                Vector3 targetTangent = _isForward ? tg : -tg;
                _rigidbody.rotation = Quaternion.LookRotation(targetTangent, targetUp);
                
                Kinematics.Normal = targetUp;
                
                _data.Time += Vector3.Dot(_rigidbody.linearVelocity, tg) * dt;
                if (_rail.Container.Spline.Closed) _data.Time = Mathf.Repeat(_data.Time, _data.Length);

                if (!_rail.Container.Spline.Closed)
                {
                    if (IsRailCooldown()) return;

                    const float THRESHOLD = 0.0005f;
                    bool outOfTime = 1 - _data.NormalizedTime < THRESHOLD || _data.NormalizedTime < THRESHOLD;
                    
                    if (outOfTime)
                    {
                        SetCooldown(0.1f);
                        StateMachine.SetState<FStateAir>();
                    }
                }
            }
        }

        public void SetRail(Rail rail)
        {
            Vector3 pos = _rigidbody.position - Actor.transform.up * rail.Radius;
            _data = new SplineData(rail.Container, pos);
            _data.EvaluateWorld(out _, out Vector3 tg, out var up, out var right);
            
            _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, up);
            _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, right);
            
            float dot = Vector3.Dot(_rigidbody.transform.forward, tg);
            _isForward = dot > 0;
            
            _rail = rail;
        }
        
        public void SetForward(bool isForward) => _isForward = isForward;

        /// <summary>
        /// We need to share data between grind states to prevent unwanted calculations
        /// </summary>
        private void Share(Rail rail, SplineData data, bool isForward)
        {
            _rail = rail;
            _data = data;
            _isForward = isForward; 
        }

        private void SetCooldown(float time)
        {
            _timer = Mathf.Abs(time);
        }
        public bool IsRailCooldown() => _timer > 0;

        public void BoostHandle()
        {
            Actor.StateMachine.GetSubState<FBoost>().BaseGroundBoost();
        }
    }
}