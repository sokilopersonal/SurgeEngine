using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.GameDocuments;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class NewModernState : CState
    {
        protected float _distance;
        protected float _yOffset;
        
        public NewModernState(Actor owner) : base(owner)
        {
            SetDirection(_actor.transform.forward);
            
            _distance = _master.distance;
            _yOffset = _master.yOffset;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            LookAxis();
            ModernSetup();
        }

        private void ModernSetup()
        {
            var actorPosition = CalculateTarget(out var targetPosition, _distance, _yOffset);

            ZLag();
            YLag();
            
            Setup(targetPosition, actorPosition);
        }

        private Vector3 CalculateTarget(out Vector3 targetPosition, float distance, float yOffset)
        {
            Quaternion horizontal = Quaternion.AngleAxis(_stateMachine.x, Vector3.up);
            Quaternion vertical = Quaternion.AngleAxis(_stateMachine.y, Vector3.right);
            Vector3 direction = horizontal * vertical * Vector3.back;
            Vector3 actorPosition = _actor.transform.position + Vector3.up * yOffset + Vector3.up * _stateMachine.yLag;
            targetPosition = actorPosition + direction * (distance + _stateMachine.zLag);
            return actorPosition;
        }

        protected virtual void LookAxis()
        {
            AutoLookDirection();
            
            var v = _actor.input.lookVector;
            _stateMachine.x += v.x + _stateMachine.xAutoLook;
            _stateMachine.y -= v.y;
            _stateMachine.y = Mathf.Clamp(_stateMachine.y, -75, 85);
        }

        private void ZLag()
        {
            Vector3 vel = _actor.kinematics.Rigidbody.linearVelocity;
            Vector3 localVel = _actor.transform.InverseTransformDirection(vel);
            float zLag = Mathf.Clamp(localVel.z * 0.075f, 0, _master.zLagMax);
            _stateMachine.zLag = Mathf.SmoothDamp(_stateMachine.zLag, zLag, ref _stateMachine.zLagVelocity, _master.zLagTime);
        }

        private void YLag()
        {
            Vector3 vel = _actor.kinematics.Rigidbody.linearVelocity;
            float yLag = Mathf.Clamp(vel.y * -0.125f, _master.yLagMin, _master.yLagMax); // min is down lag, max value is up lag
            _stateMachine.yLag = Mathf.SmoothDamp(_stateMachine.yLag, yLag, ref _stateMachine.yLagVelocity, _master.yLagTime, 5f);
        }

        protected virtual void AutoLookDirection()
        {
            if (_actor.input.IsAutoCamera())
            {
                float speed = _actor.kinematics.HorizontalSpeed;
                if (speed > 1f)
                {
                    if (NotOnTheWall())
                    {
                        float lookMod = _actor.kinematics.HorizontalSpeed / SonicGameDocument.GetDocument("Sonic")
                            .GetGroup(SonicGameDocument.PhysicsGroup)
                            .GetParameter<float>(SonicGameDocumentParams.BasePhysics_TopSpeed);
                        AutoLook(6 * Mathf.Max(0.2f, Mathf.Clamp01(lookMod)));
                    }
                }
                else
                {
                    _stateMachine.xAutoLook = 0;
                    _stateMachine.yAutoLook = 0;
                }
                
                Vector3 vel = _actor.kinematics.Rigidbody.linearVelocity;
                _stateMachine.yAutoLook = Mathf.Clamp(-vel.y * 1.25f, -5f, 5f);
            }
            else
            {
                _stateMachine.xAutoLook = 0;
                _stateMachine.yAutoLook = 0;
            }
        }

        protected void AutoLook(float multiplier)
        {
            float fwd = _actor.stats.GetForwardSignedAngle() * Time.deltaTime;
            float dot = Vector3.Dot(Vector3.Cross(_stateMachine.transform.right, Vector3.up), _actor.transform.forward);
            Vector3 vel = _actor.kinematics.Rigidbody.linearVelocity;

            if (!Mathf.Approximately(dot, -1))
            {
                _stateMachine.xAutoLook = fwd * multiplier;
            }                        
        }

        private void Setup(Vector3 targetPosition, Vector3 actorPosition)
        {
            SetPosition(targetPosition);
            SetRotation(actorPosition);
        }

        protected virtual void SetPosition(Vector3 targetPosition)
        {
            _stateMachine.position = targetPosition;
        }

        protected virtual void SetRotation(Vector3 actorPosition)
        {
            _stateMachine.rotation = Quaternion.LookRotation(actorPosition + _master.lookOffset - _stateMachine.position);
        }

        public void SetDirection(Vector3 transformForward)
        {
            Quaternion direction = Quaternion.LookRotation(transformForward, Vector3.up);
            _stateMachine.x = direction.eulerAngles.y;
            _stateMachine.y = direction.eulerAngles.x;
        }

        private bool NotOnTheWall()
        {
            return !(1 - Mathf.Abs(Vector3.Dot(_actor.transform.forward, Vector3.up)) < 0.01f);
        }
    }
}
