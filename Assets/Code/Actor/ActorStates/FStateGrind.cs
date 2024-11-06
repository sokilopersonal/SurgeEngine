using System;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters.SonicSubStates;
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
            
            if (_rail != null)
            {
                var container = _rail;
                SplineUtility.GetNearestPoint(container.Spline, 
                    SurgeMath.Vector3ToFloat3(container.transform.InverseTransformPoint(_rigidbody.position - Actor.transform.up * 1.25f)), 
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
                
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, normal);
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, tangentNormal);

                if (!Actor.stateMachine.GetSubState<FBoost>().Active)
                {
                    Vector3 slopeForce = Vector3.ProjectOnPlane(Vector3.down, Actor.transform.up) * 9;
                    _rigidbody.AddForce(slopeForce * Time.fixedDeltaTime, ForceMode.Impulse);
                }

                if (!_rail.Spline.Closed)
                {
                    float v = forward ? Mathf.Abs(t - 1) : t;
                    Debug.Log(v);
                    if (v < 0.0002f || v > 1f)
                    {
                        //_rigidbody.linearVelocity += SurgeMath.Float3ToVector3(direction) * 2f;
                        StateMachine.SetState<FStateAir>();
                    }
                }
            }
        }

        public void SetRail(SplineContainer rail)
        {
            _rail = rail;
        }

        public void BoostHandle()
        {
            if (Actor.stateMachine.GetSubState<FBoost>().Active)
            {
                _rigidbody.linearVelocity = _rigidbody.transform.forward * 25;
            }
        }
    }
}