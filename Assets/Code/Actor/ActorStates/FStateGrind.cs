using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.GameDocuments;
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

        private float _timer;
        
        public FStateGrind(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _grindGravityPower = 10f;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0.1f;

            float minSpeed = 3f;
            if (Kinematics.HorizontalSpeed < minSpeed)
            {
                _rigidbody.linearVelocity = _rigidbody.transform.forward * minSpeed;
            }

            if (StateMachine.PreviousState is FStateHoming)
            {
                _rigidbody.linearVelocity = _rigidbody.transform.forward * 37f;
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
                var container = _rail.container;
                Vector3 offset = _rigidbody.position - Actor.transform.up * (_rail.radius);
                SplineUtility.GetNearestPoint(container.Spline, 
                    SurgeMath.Vector3ToFloat3(container.transform.InverseTransformPoint(offset)), 
                    out var near, out var t);
                var normal = SurgeMath.Float3ToVector3(container.EvaluateUpVector(t));
                container.Evaluate(t, out var point, out var tangent, out var up);
                var tangentNormal = Vector3.Cross(tangent, up);
                
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
                
                if (!Actor.stateMachine.GetSubState<FBoost>().Active)
                {
                    if (Kinematics.Angle > 3f)
                    {
                        Vector3 slopeForce = Vector3.ProjectOnPlane(Vector3.down, Actor.transform.up) * _grindGravityPower;
                        _rigidbody.AddForce(slopeForce * Time.fixedDeltaTime, ForceMode.Impulse);
                    }
                }
                
                container.Evaluate(0f, out var startPoint, out _, out _);
                container.Evaluate(1f, out var endPoint, out _, out _);
                Debug.DrawRay(startPoint, Vector3.up * 2);
                Debug.DrawRay(endPoint, Vector3.up * 2);

                if (!container.Spline.Closed)
                {
                    if (_timer == 0)
                    {
                        float startDistance = Vector3.Distance(startPoint, offset);
                        float endDistance = Vector3.Distance(endPoint, offset);
                        float threshold = Mathf.Max(0.05f, 0.05f * Actor.kinematics.Rigidbody.linearVelocity.magnitude * 0.5f);

                        if (startDistance < threshold || endDistance < threshold)
                        {
                            _rail.End();
                            StateMachine.SetState<FStateAir>();
                        }
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
            FBoost boost = Actor.stateMachine.GetSubState<FBoost>();
            var phys = SonicGameDocument.GetDocument("Sonic").GetGroup(SonicGameDocument.PhysicsGroup);
            var param = boost.GetBoostEnergyGroup();
            float startForce = param.GetParameter<float>(SonicGameDocumentParams.BoostEnergy_StartSpeed);
            if (boost.Active)
            {
                if (Input.BoostPressed)
                {
                    _rigidbody.linearVelocity = _rigidbody.transform.forward * startForce / 2;
                }

                if (boost.Active)
                {
                    float maxSpeed = phys.GetParameter<float>(SonicGameDocumentParams.BasePhysics_MaxSpeed) * param.GetParameter<float>(SonicGameDocumentParams.BoostEnergy_MaxSpeedMultiplier);
                    if (Kinematics.HorizontalSpeed < maxSpeed)
                    {
                        _rigidbody.AddForce(_rigidbody.transform.forward * (param.GetParameter<float>(SonicGameDocumentParams.BoostEnergy_Force) * Time.deltaTime), ForceMode.VelocityChange);
                    }
                }
            }
        }
    }
}