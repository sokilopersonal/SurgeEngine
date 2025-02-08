using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Tools;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateGrind : FStateMove, IBoostHandler, IDamageableState
    {
        private Rail _rail;
        private Vector3 _prevTg;

        protected float _grindGravityPower;

        private float _timer;
        
        public FStateGrind(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _grindGravityPower = 10f;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0.1f;

            Actor.TryGetConfig(out HomingConfig homingConfig);

            if (StateMachine.PreviousState is FStateHoming)
            {
                _rigidbody.linearVelocity = _rigidbody.transform.forward * homingConfig.speed;
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (Input.JumpPressed)
            {
                StateMachine.SetState<FStateGrindJump>();
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
                var spline = _rail.container.Spline;
                Vector3 pos = _rigidbody.position - Actor.transform.up * _rail.radius;
                Vector3 localPos = _rail.transform.InverseTransformPoint(pos);
                SplineUtility.GetNearestPoint(spline, localPos, out var p, out var f, 12, 8);
                f *= spline.GetLength();

                SplineSample sample = new SplineSample
                {
                    pos = _rail.container.EvaluatePosition(f / spline.GetLength()),
                    tg = ((Vector3)spline.EvaluateTangent(f / spline.GetLength())).normalized,
                    up = spline.EvaluateUpVector(f / spline.GetLength())
                };
                
                if (_prevTg == Vector3.zero) _prevTg = sample.tg;
                
                Debug.DrawRay(pos, sample.up);
                Vector3 upSplinePlane = Vector3.Cross(sample.tg, Vector3.up);
                
                Kinematics.Project();
                Kinematics.Project(upSplinePlane);
                
                float sign = Mathf.Sign(Vector3.Dot(Actor.transform.forward, sample.tg));
                bool forward = Mathf.Approximately(sign, 1);
                Vector3 direction = forward ? sample.tg : -sample.tg;
                _rigidbody.rotation = Quaternion.LookRotation(direction, sample.up);
                
                _prevTg = sample.tg;

                Vector3 newPos = sample.pos;
                newPos += sample.up * _rail.radius;

                SurgeMath.SplitPlanarVector(_rigidbody.position, 
                    sample.ProjectOnUp(sample.tg).normalized, 
                    out var pLat, 
                    out var pVer);
                
                SurgeMath.SplitPlanarVector(newPos, sample.ProjectOnUp(sample.tg).normalized, 
                    out var sLat,
                    out var sVer);
                
                pLat = sLat;
                
                Vector3 slopeForce = Vector3.ProjectOnPlane(Vector3.down, Actor.transform.up) * _grindGravityPower;
                _rigidbody.AddForce(slopeForce * Time.fixedDeltaTime, ForceMode.Impulse);
                
                Kinematics.SlopePhysics();
                _rigidbody.position = pLat + pVer;

                if (!_rail.container.Spline.Closed)
                {
                    float f1 = f / spline.GetLength();
                    if (1f - f1 < 0.005f || f1 < 0.005f)
                    {
                        _rail.End();
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