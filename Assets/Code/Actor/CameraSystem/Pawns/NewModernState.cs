using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.GameDocuments;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class NewModernState : CState
    {
        private readonly float _distance;
        private readonly float _yOffset;

        public NewModernState(Actor owner) : base(owner)
        {
            _distance = 2.8f;
            _yOffset = 0.08f;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            LookAxis();
            
            var actorPosition = CalculateTarget(out var targetPosition);

            YLag();
            ZLag();
            
            Setup(targetPosition, actorPosition);
        }

        protected Vector3 CalculateTarget(out Vector3 targetPosition)
        {
            Quaternion horizontal = Quaternion.AngleAxis(_stateMachine.x, Vector3.up);
            Quaternion vertical = Quaternion.AngleAxis(_stateMachine.y, Vector3.right);
            Vector3 direction = horizontal * vertical * Vector3.back;
            Vector3 actorPosition = _actor.transform.position + Vector3.up * _yOffset + Vector3.up * _stateMachine.yLag;
            targetPosition = actorPosition + direction * (_distance + _stateMachine.zLag);
            return actorPosition;
        }

        private void LookAxis()
        {
            AutoLookDirection();
            
            var v = _actor.input.lookVector;
            _stateMachine.x += v.x + _stateMachine.xAutoLook;
            _stateMachine.y -= v.y;
            _stateMachine.y = Mathf.Clamp(_stateMachine.y, -75, 85);
        }

        private void YLag()
        {
            Vector3 vel = _actor.kinematics.Rigidbody.linearVelocity;
            float yLag = Mathf.Clamp(vel.y * -0.1f, -0.5f, 0.5f); // min is down lag, max value is up lag
            _stateMachine.yLag = Mathf.SmoothDamp(_stateMachine.yLag, yLag, ref _stateMachine.yLagVelocity, 0.1f);
        }

        private void ZLag()
        {
            Vector3 vel = _actor.kinematics.Rigidbody.linearVelocity;
            Vector3 localVel = _actor.transform.InverseTransformDirection(vel);
            float zLag = Mathf.Clamp(localVel.z * 0.075f, 0, 0.475f);
            _stateMachine.zLag = Mathf.SmoothDamp(_stateMachine.zLag, zLag, ref _stateMachine.zLagVelocity, 0.5f);
        }

        private void AutoLookDirection()
        {
            if (_actor.input.IsAutoCamera())
            {
                float speed = _actor.kinematics.HorizontalSpeed;
                if (speed > 1f)
                {
                    if (OnTheWall())
                    {
                        float fwd = _actor.stats.GetForwardSignedAngle() * Time.deltaTime;
                        float dot = Vector3.Dot(Vector3.Cross(_stateMachine.transform.right, Vector3.up), _actor.transform.forward);
                        Vector3 vel = _actor.kinematics.Rigidbody.linearVelocity;

                        if (!Mathf.Approximately(dot, -1))
                        {
                            float lookMod = _actor.kinematics.HorizontalSpeed / SonicGameDocument.GetDocument("Sonic")
                                .GetGroup(SonicGameDocument.PhysicsGroup)
                                .GetParameter<float>(SonicGameDocumentParams.BasePhysics_TopSpeed);
                            _stateMachine.xAutoLook = fwd * 7 * Mathf.Max(0.25f, Mathf.Clamp(lookMod, 0, 1f));
                        }

                        _stateMachine.yAutoLook = Mathf.Clamp(-vel.y * 1.25f, -15f, 15f);
                        _stateMachine.y = Mathf.Lerp(_stateMachine.y, _stateMachine.yAutoLook, 0.02f);
                    }
                }
                else
                {
                    _stateMachine.xAutoLook = 0;
                    _stateMachine.yAutoLook = 0;
                    _stateMachine.y = Mathf.Lerp(_stateMachine.y, _stateMachine.yAutoLook, 0.005f);
                }
            }
            else
            {
                _stateMachine.xAutoLook = 0;
                _stateMachine.yAutoLook = 0;
                _stateMachine.y = Mathf.Lerp(_stateMachine.y, _stateMachine.yAutoLook, 0.005f);
            }
        }

        protected virtual void Setup(Vector3 targetPosition, Vector3 actorPosition)
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
            _stateMachine.rotation = Quaternion.LookRotation(actorPosition - _stateMachine.position);
        }

        public void SetDirection(Vector3 transformForward)
        {
            Quaternion direction = Quaternion.LookRotation(transformForward, Vector3.up);
            _stateMachine.x = direction.eulerAngles.y;
            _stateMachine.y = 0;
        }

        private bool OnTheWall()
        {
            return !(1 - Mathf.Abs(Vector3.Dot(_actor.transform.forward, Vector3.up)) < 0.01f);
        }
    }
}
