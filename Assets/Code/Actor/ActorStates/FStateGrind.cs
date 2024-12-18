using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Tools;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateGrind : FStateMove, IBoostHandler
    {
        private Rail _rail;

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
                SplineContainer container = _rail.container;
                Vector3 offset = _rigidbody.position - Actor.transform.up * (_rail.radius);
                SplineUtility.GetNearestPoint(container.Spline, 
                    SurgeMath.Vector3ToFloat3(container.transform.InverseTransformPoint(offset)), 
                    out float3 near, out float t);
                Vector3 normal = SurgeMath.Float3ToVector3(container.EvaluateUpVector(t));
                container.Evaluate(t, out float3 point, out float3 tangent, out float3 up);
                Vector3 tangentNormal = Vector3.Cross(tangent, up);
                
                Debug.DrawRay(offset, normal, Color.yellow, 1f);
                
                float sign = Mathf.Sign(Vector3.Dot(Actor.transform.forward, tangent));
                bool forward = Mathf.Approximately(sign, 1);
                Vector3 direction = forward ? tangent : -tangent;
                
                Quaternion rot = Quaternion.LookRotation(direction, normal);
                _rigidbody.rotation = rot;
                Actor.model.root.rotation = rot;
                
                Vector3 nearPoint = container.transform.TransformPoint(near);
                nearPoint += normal * _rail.radius;
                _rigidbody.position = nearPoint;
                
                Kinematics.Normal = normal;
                Kinematics.Project();
                Kinematics.Project(tangentNormal);
                
                if (!SonicTools.IsBoost())
                {
                    if (Kinematics.Angle > 3f)
                    {
                        Vector3 slopeForce = Vector3.ProjectOnPlane(Vector3.down, Actor.transform.up) * _grindGravityPower;
                        _rigidbody.AddForce(slopeForce * Time.fixedDeltaTime, ForceMode.Impulse);
                    }
                }
                
                container.Evaluate(0f, out float3 startPoint, out _, out _);
                container.Evaluate(1f, out float3 endPoint, out _, out _);
                Debug.DrawRay(startPoint, Vector3.up * 2);
                Debug.DrawRay(endPoint, Vector3.up * 2);

                if (!container.Spline.Closed)
                {
                    Debug.Log(t);
                    if (1f - t < 0.01f || t < 0.01f)
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