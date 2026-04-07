using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility.Rails;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateGrind : FCharacterState, ISkip2D
    {
        private Rail _rail;
        private SplineData _data;
        private Vector3 _lastTangent;
        
        private bool _isForward;
        private float _timer;
        private bool Switching => Character.Animation.StateAnimator.GetCurrentAnimationState().Contains("GrindSwitch");

        protected float GravityPower;
        
        public FStateGrind(CharacterBase owner) : base(owner)
        {
            GravityPower = 10f;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (StateMachine.PreviousState is not FStateRailSwitch)
            {
                Rigidbody.linearVelocity = Vector3.ClampMagnitude(Rigidbody.linearVelocity, Character.Config.topSpeed);
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (Input.APressed)
            {
                SetCooldown(0.1f);
                StateMachine.SetState<FStateGrindJump>();
            }

            if (this is not FStateGrindSquat)
            {
                if (Input.BHeld && !Switching)
                {
                    StateMachine.SetState<FStateGrindSquat>()?.Share(_rail, _data, _isForward);
                }
            }

            if (Input.LeftBumperHeld && !Switching)
            {
                FindRailInDirection(true);
            }
            else if (Input.RightBumperHeld && !Switching)
            {
                FindRailInDirection(false);
            }

            CountCooldown(dt);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (_rail != null)
            {
                _data.EvaluateWorld(out var pos,  out var tg, out var targetUp, out var right);
                //right = Vector3.Cross(targetUp, tg).normalized;
                
                Rigidbody.linearVelocity = Vector3.ProjectOnPlane(Rigidbody.linearVelocity, targetUp);
                Rigidbody.linearVelocity = Vector3.ProjectOnPlane(Rigidbody.linearVelocity, right);
                
                Vector3 downForce = Vector3.ProjectOnPlane(Vector3.down, targetUp) * GravityPower;
                Rigidbody.AddForce(downForce * dt, ForceMode.Impulse);
                
                Vector3 newPos = pos;
                Vector3 endPos = pos + targetUp * (1 + _rail.Radius);
                Vector3 physicsTarget = newPos + Vector3.ProjectOnPlane(Rigidbody.position - newPos, right);
                Rigidbody.MovePosition(endPos);
                
                Vector3 targetTangent = _isForward ? tg : -tg;
                Rigidbody.MoveRotation(Quaternion.LookRotation(targetTangent, targetUp));
                
                Kinematics.Normal = targetUp;
                
                _data.Time += Vector3.Dot(Rigidbody.linearVelocity, tg) * dt;
                if (_rail.Container.Spline.Closed) _data.Time = Mathf.Repeat(_data.Time, _data.Length);

                if (!_rail.Container.Spline.Closed)
                {
                    if (IsRailCooldown()) return;
                    
                    bool outOfTime = _data.Time > _data.Length || _data.Time < 0f;
                    if (outOfTime)
                    {
                        SetCooldown(0.1f);
                        StateMachine.SetState<FStateAir>();
                    }
                }
            }
        }

        public void SetRail(Rail rail)
        {
            Vector3 pos = Rigidbody.position - Character.transform.up * rail.Radius;
            _data = new SplineData(rail.Container, pos);
            _data.EvaluateWorld(out _, out Vector3 tg, out var up, out var right);
            
            float dot = Vector3.Dot(Kinematics.Velocity.normalized, tg);
            _isForward = dot > 0;
            
            _rail = rail;
        }

        private void FindRailInDirection(bool isLeft)
        {
            Vector3 direction = isLeft ? -Rigidbody.transform.right : Rigidbody.transform.right;
            float dist = Character.Config.railSearchDistance;

            Vector3 searchCenter = Rigidbody.position + direction * (dist * 0.5f);
            Vector3 boxHalfExtents = new Vector3(dist * 0.5f, dist * 0.8f, dist * 0.5f);

            Collider[] hits = Physics.OverlapBox(searchCenter, boxHalfExtents, Rigidbody.rotation,
                Character.Config.railMask, QueryTriggerInteraction.Ignore);

            Rail bestRail = null;
            float bestScore = float.MaxValue;

            foreach (var hit in hits)
            {
                if (!hit.TryGetComponent(out Rail rail) || rail == _rail)
                    continue;

                var tempData = new SplineData(rail.Container, Rigidbody.position);
                tempData.EvaluateWorld(out var railPos, out var tangent, out var up, out var right);

                Vector3 toRail = railPos - Rigidbody.position;
                float lateralDist = Vector3.Dot(toRail, direction);

                if (lateralDist < 0.5f)
                    continue;

                float verticalDiff = Mathf.Abs(toRail.y);

                float tangentAlignment = Mathf.Abs(Vector3.Dot(tangent.normalized, Rigidbody.transform.forward));

                float forwardDistance = Vector3.Dot(toRail, Rigidbody.transform.forward);
                float score = toRail.magnitude
                    - (tangentAlignment * 2f) + (verticalDiff * 0.5f) - (Mathf.Max(0, forwardDistance) * 0.3f);

                Debug.DrawLine(Rigidbody.position, railPos, Color.yellow);

                if (score < bestScore && lateralDist < dist)
                {
                    bestScore = score;
                    bestRail = rail;
                }
            }

            if (bestRail != null)
            {
                Debug.Log($"Found rail {bestRail.name}");

                var splineData = new SplineData(bestRail.Container, Rigidbody.position);
                splineData.EvaluateWorld(out var pos, out var tangent, out var up, out _);
                Vector3 nextPos = pos + up * (1 + bestRail.Radius);

                SetCooldown(0.1f);
                Vector3 savedVelocity = Rigidbody.linearVelocity;

                StateMachine.GetState<FStateRailSwitch>()?.Set(Rigidbody.position, nextPos, bestRail, savedVelocity, isLeft);
                StateMachine.SetState<FStateRailSwitch>();
            }
        }

        private void CountCooldown(float dt)
        {
            if (_timer > 0) _timer -= dt;
            else _timer = 0;
        }

        public void SetForward(bool isForward) => _isForward = isForward;

        /// <summary>
        /// Share data between grind states to prevent unwanted calculations
        /// </summary>
        private void Share(Rail rail, SplineData data, bool isForward)
        {
            _rail = rail;
            _data = data;
            _isForward = isForward; 
        }

        private void SetCooldown(float time)
        {
            _timer = Mathf.Abs(time);
        }
        
        public bool IsRailCooldown() => _timer > 0;
    }
}