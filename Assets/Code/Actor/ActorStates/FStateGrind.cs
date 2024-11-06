using System;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.SonicSubStates.Boost;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Parameters
{
    public class FStateGrind : FStateMove, IBoostHandler
    {
        private SplineContainer _rail;
        
        public FStateGrind(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (Common.CheckForRail(out var hit, out _))
            {
                Kinematics.Normal = hit.normal;
                Debug.Log(Kinematics.Normal);
            }
            
            if (_rail != null)
            {
                var container = _rail;
                SplineUtility.GetNearestPoint(container.Spline, SurgeMath.Vector3ToFloat3(container.transform.InverseTransformPoint(_rigidbody.position)), out var near, out var t);
                container.Evaluate(t, out var point, out var tangent, out var up);
                var planeNormal = Vector3.Cross(tangent, up);

                float minSpeed = 3;
                if (Kinematics.HorizontalSpeed < minSpeed)
                {
                    _rigidbody.AddForce(_rigidbody.transform.forward * (Time.fixedDeltaTime * minSpeed), ForceMode.Impulse);
                }
                
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, planeNormal);
                
                Vector3 nearPoint = container.transform.TransformPoint(near);
                _rigidbody.position = nearPoint + Kinematics.Normal * 1;
                
                Quaternion rot = Quaternion.LookRotation(tangent, Kinematics.Normal);
                _rigidbody.rotation = rot;

                if (!_rail.Spline.Closed)
                {
                    if (Math.Abs(t - 1) < 0.01f)
                    {
                        Actor.transform.position += Actor.transform.forward * 0.25f;
                        StateMachine.SetState<FStateAir>();
                    }
                }
                
                Vector3 vel = _rigidbody.linearVelocity;
                vel.y = 0;
                _rigidbody.linearVelocity = vel;
            }
        }

        public void SetRail(SplineContainer rail)
        {
            _rail = rail;
        }

        public void BoostHandle()
        {
            if (Kinematics.HorizontalSpeed < 40)
            {
                _rigidbody.AddForce(_rigidbody.transform.forward * (Time.fixedDeltaTime * 15), ForceMode.Impulse);
            }
        }
    }
}