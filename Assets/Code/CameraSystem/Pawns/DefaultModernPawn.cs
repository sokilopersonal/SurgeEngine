using System;
using System.Collections;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    public class DefaultModernPawn : CameraPawn
    {
        private ActorCamera _actorCamera;
        
        protected float _x;
        protected float _y;
        private float _collisionDistance;

        private bool _autoActive;
        private Vector3 _tempFollowPoint;
        private Vector3 _tempLookPoint;
        protected float _tempY;
        protected float _tempZ;
        private float _tempTime;
        
        private Vector2 _autoLookDirection;

        private CameraParameters _currentParameters;

        private float _followPower;
        private float _fov;
        private float _distance;
        private float _timeToStartFollow;

        private Coroutine _paramCoroutine;

        private void Start()
        {
            _actorCamera = actor.camera;
            _currentParameters = _actorCamera.parameters[0];

            _tempY = _actorCamera.target.position.y;
            _tempTime = _actorCamera.yFollowTime;
            
            _followPower = _currentParameters.followPower;
            _timeToStartFollow = _currentParameters.timeToStartFollow;
            _distance = _currentParameters.distance;
            _fov = _currentParameters.fov;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            SetRotationAxis(actor.transform.forward);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            actor.stateMachine.GetSubState<FBoost>().OnActiveChanged += OnBoostActivate;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            actor.stateMachine.GetSubState<FBoost>().OnActiveChanged -= OnBoostActivate;
        }
        
        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            GetLook();
            AutoFollow();
            Following();
            Collision();
            LookAt();
            SetFieldOfView(_fov);
        }

        protected void Following()
        {
            if (actor.stats.isGrounded)
            {
                _tempTime = Mathf.Lerp(_tempTime, 0, SurgeMath.Smooth(1 - 0.965f));
            }
            else if (actor.stats.isInAir)
            {
                _tempTime = Mathf.Lerp(_tempTime, _actorCamera.yFollowTime, SurgeMath.Smooth(1f));
            }

            float yPos = _actorCamera.target.position.y;
            _tempY = Mathf.Lerp(_tempY, yPos, SurgeMath.Smooth(1 - _tempTime));
            _tempY = Mathf.Clamp(_tempY, yPos - 0.75f, yPos + 0.5f);
            _tempFollowPoint = _actorCamera.target.position;
            _tempFollowPoint.y = _tempY;

            float speed = actor.stats.currentSpeed;
            float zLagMod = Mathf.Lerp(0.2f, 0.1f, speed / actor.stats.moveParameters.topSpeed);
            float zLag = speed * zLagMod;
            zLag = Mathf.Clamp(zLag, 0, _actorCamera.zLagMax);
            _tempZ = Mathf.Lerp(_tempZ, zLag, _actorCamera.zLagSmoothness * Time.deltaTime);
        }

        protected virtual void GetLook()
        {
            var lookVector = actor.input.lookVector;
            _x += lookVector.x + _autoLookDirection.x;
            _y -= lookVector.y;
            _y = Mathf.Clamp(_y, -65, 65);
        }

        protected virtual void AutoFollow()
        {
            if (Common.InDelayTime(actor.input.GetLastLookInputTime(), _timeToStartFollow))
            {
                _autoActive = true;
                _followPower = _currentParameters.followPower;
                
                if (actor.stats.currentSpeed > 1f)
                {
                    if (!(1 - Mathf.Abs(Vector3.Dot(actor.transform.forward, Vector3.up)) < 0.01f))
                    {
                        float fwd = actor.stats.GetForwardSignedAngle() * Time.deltaTime;
                        _autoLookDirection.x = fwd * _followPower; 
                        if (actor.stats.isInAir)
                        {
                            Vector3 vel = Vector3.ClampMagnitude(actor.rigidbody.linearVelocity, 2f);
                            vel.y = Mathf.Lerp(vel.y, Mathf.Clamp(vel.y, -0.5f, 0.15f), SurgeMath.Smooth(1f));
                            _actorCamera.lookOffset.y = Mathf.Lerp(_actorCamera.lookOffset.y, vel.y, SurgeMath.Smooth(0.05f));
                        }
                        else
                        {
                            _actorCamera.lookOffset.y = Mathf.Lerp(_actorCamera.lookOffset.y, 0f, 4f * Time.deltaTime);
                        }
                    }
                }
                else
                {
                    _autoLookDirection.x = 0;
                }

                if (actor.stateMachine.CurrentState is FStateGround or FStateIdle or FStateDrift or FStateSliding)
                {
                    _autoLookDirection.y = 5f * (actor.stateMachine.GetSubState<FBoost>().Active ? 1.75f : 1f);
                    _autoLookDirection.y -= actor.stats.currentVerticalSpeed * 1.25f;
                    
                    if (Mathf.Approximately(actor.stats.groundAngle, 90))
                    {
                        _autoLookDirection.y = 0f;
                    }
                }
                else
                {
                    _autoLookDirection.y = 0f;
                }
                
                _y = Mathf.Lerp(_y, _autoLookDirection.y, 2.25f * Time.deltaTime);
            }
            else
            {
                _autoActive = false;
                _autoLookDirection.x = 0;
                _actorCamera.lookOffset.y = Mathf.Lerp(_actorCamera.lookOffset.y, 0f, SurgeMath.Smooth(0.1f));
            }
        }

        protected void LookAt()
        {
            if (_autoActive)
            {
                if (actor.stateMachine.CurrentState is FStateDrift)
                {
                    Vector3 vel = Vector3.ClampMagnitude(actor.rigidbody.linearVelocity, 2.5f);
                    _tempLookPoint = Vector3.Lerp(_tempLookPoint, vel, 16f * Time.deltaTime);
                    _tempLookPoint.y = 0;
                }
                else
                {
                    _tempLookPoint = Vector3.Lerp(_tempLookPoint, Vector3.zero, 4f * Time.deltaTime);
                }
            }
            else
            {
                _tempLookPoint = Vector3.Lerp(_tempLookPoint, Vector3.zero, 4f * Time.deltaTime);
            }
            
            _tempFollowPoint += _tempLookPoint;
            SetRotation(_tempFollowPoint);
        }

        protected void Collision()
        {
            var ray = new Ray(_actorCamera.target.position + Vector3.forward * _tempZ, -_cameraTransform.forward);
            float radius = _actorCamera.collisionRadius;
            var maxDistance = _distance + _tempZ;

            float result = Physics.SphereCast(ray, radius, out RaycastHit hit,
                maxDistance, _actorCamera.collisionMask, QueryTriggerInteraction.Ignore)
                ? hit.distance
                : _distance + _tempZ;
            
            SetPosition(GetTarget(result - _tempZ));
        }

        public virtual void SetPosition(Vector3 pos)
        {
            _cameraTransform.position = pos;
        }

        public virtual void SetRotation(Vector3 pos)
        {
            _cameraTransform.rotation = Quaternion.LookRotation(pos - _cameraTransform.position);
        }

        public void SetRotationAxis(Vector3 dir)
        {
            Quaternion rotation = Quaternion.LookRotation(dir);
            SetRotationValues(rotation.eulerAngles.y, rotation.eulerAngles.x);
        }

        public void SetRotationValues(float x, float y)
        {
            _x = x;
            _y = y;
        }

        protected virtual void SetFieldOfView(float fov)
        {
            _camera.fieldOfView = fov;
        }

        public void SetTempZ(float z) => _tempZ = z;

        public void SetTempY(float y) => _tempY = y;

        private Vector3 GetTarget(float distance)
        {
            Vector3 targetPosition = _tempFollowPoint;
            Vector3 v = targetPosition - Quaternion.Euler(_y, _x, 0) *
                (Vector3.forward * distance + Vector3.forward * _tempZ);
            return v;
        }

        private void OnBoostActivate(FSubState obj, bool value)
        {
            if (obj is FBoost && value)
            {
                TransitionTo("BoostOut", _ =>
                {
                    TransitionTo("BoostIn");
                });
            }
            else
            {
                TransitionTo("Default");
            }
        }

        private void TransitionTo(string parameter, Action<CameraParameters> callback = null)
        {
            if (_paramCoroutine != null)
            {
                _actorCamera.StopCoroutine(_paramCoroutine);
            }

            var param = actor.camera.parameters.Find(x => x.name == parameter);
            _paramCoroutine = _actorCamera.StartCoroutine(ChangeParametersCoroutine(param, callback));
        }

        private IEnumerator ChangeParametersCoroutine(CameraParameters target, Action<CameraParameters> callback)
        {
            float tDistance = 0f;
            float tFov = 0f;

            float startDistance = _distance;
            float startFov = _fov;
            
            float targetDistance = target.distance;
            float targetFov = target.fov;

            while (tDistance < target.distanceDuration || tFov < target.fovDuration)
            {
                if (tDistance < target.distanceDuration)
                {
                    tDistance += Time.deltaTime;
                    _distance = Mathf.Lerp(startDistance, targetDistance, Easings.Get(target.distanceEasing, tDistance / target.distanceDuration));
                }

                if (tFov < target.fovDuration)
                {
                    tFov += Time.deltaTime;
                    _fov = Mathf.Lerp(startFov, targetFov, Easings.Get(target.fovEasing, tFov / target.fovDuration));
                }

                yield return null;
            }

            _distance = targetDistance;
            _fov = targetFov;

            _currentParameters = target;
            callback?.Invoke(target);
        }
    }
}