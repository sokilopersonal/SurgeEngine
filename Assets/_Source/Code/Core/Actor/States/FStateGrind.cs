using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.Actor.System.Characters.Sonic;
using SurgeEngine.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateGrind : FStateMove, IBoostHandler
    {
        private Rail _rail;
        private Vector3 _prevTg;

        protected float _grindGravityPower;

        private float _timer;
        
        public FStateGrind(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _grindGravityPower = 10f;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0.1f;

            if (StateMachine.PreviousState is FStateHoming)
            {
                _rigidbody.linearVelocity = _rigidbody.transform.forward * (Actor as Sonic).homingConfig.speed;
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (Input.JumpPressed)
            {
                StateMachine.SetState<FStateGrindJump>();
                _rigidbody.position += _rigidbody.transform.up * 0.05f;
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

                SplineSample sample = new SplineSample
                {
                    pos = _rail.Container.EvaluatePosition(f),
                    tg = ((Vector3)spline.EvaluateTangent(f)).normalized,
                    up = spline.EvaluateUpVector(f)
                };
                
                if (_prevTg == Vector3.zero) _prevTg = sample.tg;
                
                Vector3 upSplinePlane = Vector3.Cross(sample.tg, sample.up);
                
                Kinematics.Project(sample.up);
                Kinematics.Project(upSplinePlane);
                
                float sign = Mathf.Sign(Vector3.Dot(Actor.transform.forward, sample.tg));
                bool forward = Mathf.Approximately(sign, 1);
                Vector3 direction = forward ? sample.tg : -sample.tg;
                _rigidbody.rotation = Quaternion.LookRotation(direction, sample.up);
                
                _prevTg = sample.tg;

                Vector3 newPos = sample.pos;
                newPos += sample.up * (1 + _rail.Radius);

                Vector3 planeNormal = Vector3.ProjectOnPlane(sample.tg, sample.up);
                SurgeMath.SplitPlanarVector(_rigidbody.position, 
                    planeNormal, 
                    out _, 
                    out var pVer);
                
                SurgeMath.SplitPlanarVector(newPos, planeNormal, 
                    out var sLat,
                    out _);
                
                Vector3 slopeForce = Vector3.ProjectOnPlane(Vector3.down, sample.up) * _grindGravityPower;
                _rigidbody.AddForce(slopeForce * Time.fixedDeltaTime, ForceMode.Impulse);
                
                _rigidbody.position = sLat + pVer;

                if (!_rail.Container.Spline.Closed)
                {
                    Ray ray = new Ray(pos, -sample.up);
                    float dist = 1.5f;
                    Debug.DrawRay(ray.origin, ray.direction * dist, Color.red);
                    if (!Physics.SphereCast(ray, 0.25f, dist, 1 << _rail.gameObject.layer))
                    {
                        _rigidbody.position += _rigidbody.transform.forward * 0.25f;
                        StateMachine.SetState<FStateAir>();
                    }
                }
            }
        }

        public void SetRail(Rail rail)
        {
            _rail = rail;
        }

        public void BoostHandle()
        {
            Actor.stateMachine.GetSubState<FBoost>().BaseGroundBoost();
        }
    }
}