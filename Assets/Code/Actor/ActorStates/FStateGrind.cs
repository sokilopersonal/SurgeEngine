using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.SonicSubStates.Boost;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Parameters
{
    public class FStateGrind : FStateMove, IBoostHandler
    {
        private Rail _rail;

        protected float _grindGravityPower;
        
        public FStateGrind(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _grindGravityPower = 10f;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            float minSpeed = 3f;
            if (Kinematics.HorizontalSpeed < minSpeed)
            {
                _rigidbody.linearVelocity = _rigidbody.transform.forward * minSpeed;
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
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (_rail != null)
            {
                var container = _rail.container;
                Vector3 offset = _rigidbody.position - Actor.transform.up * (1f + _rail.radius);
                SplineUtility.GetNearestPoint(container.Spline, 
                    SurgeMath.Vector3ToFloat3(container.transform.InverseTransformPoint(offset)), 
                    out var near, out var t);
                container.Evaluate(t, out var point, out var tangent, out var up);
                var tangentNormal = Vector3.Cross(tangent, up);
                var normal = SurgeMath.Float3ToVector3(up);

                float sign = Mathf.Sign(Vector3.Dot(Actor.transform.forward, tangent));
                bool forward = Mathf.Approximately(sign, 1);
                Vector3 direction = forward ? tangent : -tangent;
                
                Quaternion rot = Quaternion.LookRotation(direction, Actor.transform.up);
                _rigidbody.rotation = rot;
                
                Vector3 nearPoint = container.transform.TransformPoint(near);
                nearPoint += Actor.transform.up * 1.25f;
                _rigidbody.position = nearPoint;
                
                Kinematics.Normal = normal;
                Kinematics.Project();
                Kinematics.Project(tangentNormal);

                Debug.Log(Kinematics.Angle);
                if (!Actor.stateMachine.GetSubState<FBoost>().Active)
                {
                    if (Kinematics.Angle > 3f)
                    {
                        Vector3 slopeForce = Vector3.ProjectOnPlane(Vector3.down, Actor.transform.up) * _grindGravityPower;
                        _rigidbody.AddForce(slopeForce * Time.fixedDeltaTime, ForceMode.Impulse);
                    }
                }

                if (!container.Spline.Closed)
                {
                    float v = forward ? Mathf.Abs(t - 1) : t;
                    bool end = v is < 0.002f or > 0.99875f;
                    if (end)
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
            if (Actor.stateMachine.GetSubState<FBoost>().Active)
            {
                _rigidbody.AddForce(_rigidbody.transform.forward * 35, ForceMode.Acceleration);
            }
        }
    }
}