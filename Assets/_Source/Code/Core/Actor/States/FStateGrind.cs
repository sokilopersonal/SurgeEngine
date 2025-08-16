using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Mobility.Rails;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateGrind : FCharacterState
    {
        private Rail _rail;
        private SplineData _data;
        private Vector3 _lastTangent;
        
        private bool _isForward;
        private float _timer;

        protected float gravityPower;
        
        public FStateGrind(CharacterBase owner) : base(owner)
        {
            gravityPower = 10f;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (StateMachine.PreviousState is not FStateRailSwitch)
            {
                Rigidbody.linearVelocity = Vector3.ClampMagnitude(Rigidbody.linearVelocity, character.Config.topSpeed);
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
                if (Input.BHeld)
                {
                    StateMachine.SetState<FStateGrindSquat>()?.Share(_rail, _data, _isForward);
                }
            }

            if (Input.LeftBumperHeld)
            {
                FindRailInDirection(true);
            }
            else if (Input.RightBumperHeld)
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
                
                Debug.DrawRay(pos, tg, Color.white);
                Debug.DrawRay(pos, targetUp, Color.green);
                Debug.DrawRay(pos, right, Color.yellow);
                
                Rigidbody.linearVelocity = Vector3.ProjectOnPlane(Rigidbody.linearVelocity, targetUp);
                Rigidbody.linearVelocity = Vector3.ProjectOnPlane(Rigidbody.linearVelocity, right);
                
                Vector3 downForce = Vector3.ProjectOnPlane(Vector3.down, targetUp) * gravityPower;
                Rigidbody.AddForce(downForce * dt, ForceMode.Impulse);
                
                Vector3 endPos = pos + targetUp * (1 + _rail.Radius);
                Rigidbody.position = endPos;
                
                Vector3 targetTangent = _isForward ? tg : -tg;
                Rigidbody.rotation = Quaternion.LookRotation(targetTangent, targetUp);
                
                Kinematics.Normal = targetUp;
                
                _data.Time += Vector3.Dot(Rigidbody.linearVelocity, tg) * dt;
                if (_rail.Container.Spline.Closed) _data.Time = Mathf.Repeat(_data.Time, _data.Length);

                if (!_rail.Container.Spline.Closed)
                {
                    if (IsRailCooldown()) return;

                    const float THRESHOLD = 0.0002f;
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
            Vector3 pos = Rigidbody.position - character.transform.up * rail.Radius;
            _data = new SplineData(rail.Container, pos);
            _data.EvaluateWorld(out _, out Vector3 tg, out var up, out var right);
            
            float dot = Vector3.Dot(Kinematics.Velocity.normalized, tg);
            _isForward = dot > 0;
            
            _rail = rail;
        }

        private void FindRailInDirection(bool isLeft)
        {
            Vector3 direction = isLeft ? -Rigidbody.transform.right : Rigidbody.transform.right;
            float mult = Vector3.ClampMagnitude(Rigidbody.linearVelocity / 32f, 1f).magnitude;
            Vector3 forward = Rigidbody.transform.forward * mult;
            Vector3 predictedPoint = Rigidbody.position + Vector3.Normalize(Vector3.Lerp(direction, forward, 0.5f));

            float dist = character.Config.railSearchDistance;
            Vector3 rayDirection = (predictedPoint - Rigidbody.position).normalized;
            
            Debug.DrawRay(Rigidbody.position, rayDirection * dist, Color.green);
            
            if (Physics.SphereCast(Rigidbody.position, 1.45f, rayDirection, out var hit, dist))
            {
                if (hit.collider.TryGetComponent(out Rail rail) && rail != _rail && hit.distance <= dist / 2)
                {
                    Debug.Log($"Found rail {rail.name}");
                    
                    var splineData = new SplineData(rail.Container, Rigidbody.position);
                    splineData.EvaluateWorld(out var pos, out var tangent, out var up, out _);
                    Vector3 nextPos = pos + up * (1 + rail.Radius);
                    
                    SetCooldown(0.1f);
                    Vector3 savedVelocity = Rigidbody.linearVelocity;

                    StateMachine.GetState<FStateRailSwitch>()?.Set(Rigidbody.position, nextPos, rail, savedVelocity, isLeft);
                    StateMachine.SetState<FStateRailSwitch>();
                }
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