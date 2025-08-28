using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Interfaces;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateQuickstep : FCharacterState, IStateTimeout
    {
        public float Timeout { get; set; }
        
        private QuickstepDirection _direction;
        private float _timer;
        public bool IsRun { get; private set; }

        private Vector3 _snapStartPos;
        private Vector3 _snapTargetPos;
        private Vector3 _snapTangent;
        private Vector3 _lastTangent;
        private float _snapDot;
        
        private Vector3 _snapVelocity;
        private bool _isSnapping;

        private readonly QuickStepConfig _config;

        public FStateQuickstep(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0;
            _isSnapping = false;
            
            Timeout = _config.delay;
            
            float speed = !IsRun ? _config.force : _config.runForce;
            var sideDir = _direction == QuickstepDirection.Left ? -speed : speed;
            if (Kinematics.PathDash == null)
            {
                SetSideVelocity(sideDir);
            }
            else 
            {
                bool snapped = SnapToSpline();
                if (!snapped)
                {
                    SetSideVelocity(sideDir);
                }
            }
            
            if (StateMachine.PreviousState is FStateSlide)
            {
                Rigidbody.linearVelocity += Rigidbody.transform.forward * 8f; // TODO: Move this QSS value to config
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _timer += dt / (!IsRun ? _config.duration : _config.runDuration);
            
            if (_isSnapping)
            {
                float t = Mathf.Clamp01(_timer);
                _snapStartPos += _snapVelocity * dt;
                _snapTargetPos += _snapVelocity * dt;
                Vector3 up = Rigidbody.transform.up;
                
                var pos = Vector3.Lerp(_snapStartPos, _snapTargetPos, Easings.Get(Easing.InOutSine, t));
                pos.y = Rigidbody.position.y;
                Rigidbody.MovePosition(pos);
                    
                var tg = _snapTangent;
                tg *= Mathf.Sign(_snapDot);
                
                var rot = Quaternion.LookRotation(tg, up);
                Rigidbody.MoveRotation(rot);
                
                Kinematics.RotateSnapNormal(up);

                if (t >= 1f)
                {
                    var vel = Rigidbody.linearVelocity;
                    float horizSpeed = Vector3.Dot(vel, tg.normalized);
                    Rigidbody.linearVelocity = tg.normalized * horizSpeed + Vector3.Project(vel, up);
                    
                    _isSnapping = false;
                }
                return;
            }
            
            if (_timer >= 1f)
            {
                SetSideVelocity(0);
                if (IsRun) StateMachine.SetState<FStateGround>();
                else StateMachine.SetState<FStateIdle>();
            }
        }

        private bool SnapToSpline()
        {
            var container = Kinematics.PathDash.Spline.Container;
            var splines = container.Splines;
            if (splines.Count == 0) return false;

            float ignoreDistance = 1f;
            float ignoreSqr = ignoreDistance * ignoreDistance;
            Vector3 worldPos = Rigidbody.position - Rigidbody.transform.up * 0.5f;;
            Vector3 localPos = container.transform.InverseTransformPoint(worldPos);

            float minDist = float.MaxValue;
            Spline bestSpline = null;
            float bestT = 0f;
            float dirSign = _direction == QuickstepDirection.Right ? 1f : -1f;
            Vector3 rightAxis = Rigidbody.transform.right;

            foreach (var spline in splines)
            {
                SplineUtility.GetNearestPoint(spline, localPos, out _, out var t);
                Vector3 nearestWorld = container.transform.TransformPoint(spline.EvaluatePosition(t));
                Vector3 toNearest = nearestWorld - worldPos;
                if (Vector3.Dot(toNearest, rightAxis) * dirSign <= 0f) continue;
                float dSqr = toNearest.sqrMagnitude;
                if (dSqr < ignoreSqr) continue;
                if (dSqr < minDist)
                {
                    minDist = dSqr;
                    bestSpline = spline;
                    bestT = t;
                }
            }

            if (bestSpline == null) return false;

            _snapStartPos = worldPos;
            _snapTargetPos = container.transform.TransformPoint(bestSpline.EvaluatePosition(bestT));
            _snapTangent = container.transform.TransformDirection(bestSpline.EvaluateTangent(bestT));
            _snapDot = Vector3.Dot(Rigidbody.transform.forward, _snapTangent);
            
            Debug.DrawLine(worldPos, _snapTargetPos, Color.red, 10);
            _snapVelocity = Kinematics.Velocity;
            _timer = 0;
            _isSnapping = true;
            return true;
        }
        
        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            var config = character.Config;
            float distance = config.EvaluateCastDistance(config.castDistanceCurve.Evaluate(Kinematics.Speed / config.topSpeed));
            if (Kinematics.CheckForGround(out var hit, castDistance: distance))
            {
                bool predicted = Kinematics.CheckForPredictedGround(dt, distance, 4);
                
                if (predicted) Kinematics.Snap(hit.point, Kinematics.Normal);
                Kinematics.Project();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        private void SetSideVelocity(float sideDir)
        {
            var localVel = Rigidbody.transform.InverseTransformDirection(Rigidbody.linearVelocity);
            localVel.x = sideDir;
            Rigidbody.linearVelocity = Rigidbody.transform.TransformDirection(localVel);
        }
        
        public FStateQuickstep SetDirection(QuickstepDirection direction)
        {
            // Invert QS direction if we are looking in the opposite of player's forward
            float dot = Vector3.Dot(Rigidbody.transform.forward, character.Camera.GetCameraTransform().forward);
            if (direction == QuickstepDirection.Left && dot < 0)
            {
                direction = QuickstepDirection.Right;
            }
            else if (direction == QuickstepDirection.Right && dot < 0)
            {
                direction = QuickstepDirection.Left;
            }
            
            _direction = direction;
            return this;
        }

        public FStateQuickstep SetRun(bool isRun)
        {
            IsRun = isRun;
            return this;
        }

        public QuickstepDirection GetDirection()
        {
            return _direction;
        }
    }
    
    public enum QuickstepDirection
    {
        Left = -1,
        Right = 1
    }
}