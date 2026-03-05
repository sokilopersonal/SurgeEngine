using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Interfaces;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes;
using SurgeEngine.Source.Code.Infrastructure.Config.Sonic;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic
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
            var pathForward = Kinematics.PathForward;
            var pathDash = Kinematics.PathDash;
            if (pathForward == null && pathDash == null)
            {
                SetSideVelocity(sideDir);
            }
            else 
            {
                if (IsRun)
                {
                    ChangeMode3DData quickstepPath = null;
                    if (pathForward != null && pathForward.Tag == SplineTag.Quickstep)
                    {
                        quickstepPath = pathForward;
                    }
                    else if (pathDash != null && pathDash.Tag == SplineTag.Quickstep)
                    {
                        quickstepPath = pathDash;
                    }

                    if (quickstepPath != null)
                    {
                        bool snapped = SnapToSpline(quickstepPath);
                        if (!snapped)
                        {
                            SetSideVelocity(sideDir);
                        }
                    }
                    else
                    {
                        SetSideVelocity(sideDir);
                    }
                }
                else
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

        private bool SnapToSpline(ChangeMode3DData pathData)
        {
            var container = pathData.Spline.Container;
            var splines = container.Splines;
            
            if (splines.Count < 2) 
            {
                Debug.LogWarning("Quickstep: Container has less than 2 splines.");
                return false;
            }

            if (splines.Count > 2)
            {
                Debug.LogWarning("Quickstep: Container has more than 2 splines.");
                return false;
            }

            const float ignoreDistance = 1f;
            const float ignoreSqr = ignoreDistance * ignoreDistance;
            const float blend = 0.7f;

            Vector3 worldPos = Rigidbody.position - Rigidbody.transform.up * 0.5f;
            Vector3 localPos = container.transform.InverseTransformPoint(worldPos);

            Spline leftSpline = splines[0];
            Spline rightSpline = splines[1];

            SplineUtility.GetNearestPoint(leftSpline, localPos, out _, out var leftT);
            SplineUtility.GetNearestPoint(rightSpline, localPos, out _, out var rightT);

            var potentialTargets = new (Vector3 pos, Vector3 tangent, float sqrDist, bool isValid)[3];

            Vector3 leftPos = container.transform.TransformPoint(leftSpline.EvaluatePosition(leftT));
            leftPos.y = worldPos.y;
            potentialTargets[0] = (leftPos, container.transform.TransformDirection(leftSpline.EvaluateTangent(leftT)), (leftPos - worldPos).sqrMagnitude, false);

            Vector3 rightPos = container.transform.TransformPoint(rightSpline.EvaluatePosition(rightT));
            rightPos.y = worldPos.y;
            potentialTargets[2] = (rightPos, container.transform.TransformDirection(rightSpline.EvaluateTangent(rightT)), (rightPos - worldPos).sqrMagnitude, false);

            Vector3 centerPos = (leftPos + rightPos) * 0.5f;
            Vector3 centerTangent = (potentialTargets[0].tangent + potentialTargets[2].tangent).normalized;
            potentialTargets[1] = (centerPos, centerTangent, (centerPos - worldPos).sqrMagnitude, false);

            Vector3 rightAxis = Rigidbody.transform.right;
            float dirSign = _direction == QuickstepDirection.Right ? 1f : -1f;
            
            float bestDist = float.MaxValue;
            int bestIndex = -1;
            
            for (int i = 0; i < 3; i++)
            {
                var candidate = potentialTargets[i];
                if (candidate.sqrDist < ignoreSqr) continue;
                
                Vector3 toPoint = candidate.pos - worldPos;
                float dot = Vector3.Dot(toPoint, rightAxis) * dirSign;
                float minDot = (i == 1) ? 0f : 0.3f;

                if (dot > minDot)
                {
                    if (candidate.sqrDist < bestDist)
                    {
                        bestDist = candidate.sqrDist;
                        bestIndex = i;
                    }
                }
            }

            if (bestIndex == -1) return false;

            var target = potentialTargets[bestIndex];
            _snapStartPos = worldPos;

            float blendFactor = (bestIndex == 1) ? 1.0f : blend;
            _snapTargetPos = Vector3.Lerp(_snapStartPos, target.pos, blendFactor);
            
            _snapTangent = target.tangent;
            _snapDot = Vector3.Dot(Rigidbody.transform.forward, _snapTangent);
            
            _snapVelocity = Kinematics.Velocity;
            _timer = 0;
            _isSnapping = true;
            
            return true;
        }
        
        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            var config = Character.Config;
            float distance = config.EvaluateCastDistance(config.castDistanceCurve.Evaluate(Kinematics.Speed / config.topSpeed));
            if (Kinematics.CheckForGround(out var hit, castDistance: distance))
            {
                bool predicted = Kinematics.CheckForPredictedGround(dt, distance, 4);
                
                if (predicted) Kinematics.Snap(hit.point, Kinematics.Normal);
                Kinematics.ProjectOnNormal();
            }
            else
            {
                if ((Kinematics.PathForward != null && Kinematics.PathForward.Tag == SplineTag.Quickstep) ||
                    (Kinematics.PathDash != null && Kinematics.PathDash.Tag == SplineTag.Quickstep))
                {
                    float speed = _config.runForce;
                    var sideDir = _direction == QuickstepDirection.Left ? -speed : speed;
                    SetSideVelocity(sideDir);
                }
                
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
            float dot = Vector3.Dot(Rigidbody.transform.forward, Character.Camera.GetCameraTransform().forward);
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