using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility.Rails;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateRailSwitch : FCharacterState
    {
        private Vector3 _start;
        private Vector3 _end;
        private float _switchTimer;
        private Vector3 _savedVelocity;
        private Vector3 _lastTangent;
        private Rail _targetRail;
        private SplineData _data;

        public bool IsLeft { get; private set; }
        
        public FStateRailSwitch(CharacterBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();

            _switchTimer = 0;

            Kinematics.IsKinematic = true;
        }

        public override void OnExit()
        {
            base.OnExit();

            Kinematics.IsKinematic = false;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
    
            _data.EvaluateWorld(out var pos, out var tg, out var up, out var right);

            if (tg == Vector3.zero)
            {
                Kinematics.IsKinematic = false;
                Rigidbody.linearVelocity = _savedVelocity;
                StateMachine.SetState<FStateAir>();
                return;
            }
            
            Vector3 vel = _savedVelocity;
            float dot = Vector3.Dot(tg, vel.normalized);
            Vector3 newDir = tg * (vel.magnitude * dot);
    
            float t = _switchTimer;
            _start += newDir * dt;
            _end += newDir * dt;
            Vector3 midPoint = (_start + _end) * 0.5f + Vector3.up * 4f;
            Vector3 p1 = Vector3.Lerp(_start, midPoint, t);
            Vector3 p2 = Vector3.Lerp(midPoint, _end, t);
            Rigidbody.MovePosition(Vector3.Lerp(p1, p2, Character.Config.railSwitchCurve.Evaluate(t)));
            Rigidbody.MoveRotation(Quaternion.LookRotation(tg * dot, up));

            _lastTangent = tg;
            
            _data.Time += Vector3.Dot(tg, _savedVelocity) * dt;

            _switchTimer += dt / 0.68f;
            _switchTimer = Mathf.Clamp01(_switchTimer);

            if (_switchTimer >= 1f)
            {
                Kinematics.IsKinematic = false;
                if (!Physics.Raycast(new Ray(Rigidbody.position, Vector3.down), out var hit, 2f, Character.Config.railMask))
                {
                    Vector3 vertical = p2 - p1;
                    vertical.x *= 2f;
                    vertical.z *= 2f;
                    Vector3 newVel = newDir + vertical;
                    Rigidbody.linearVelocity = Quaternion.FromToRotation(_lastTangent, tg) * newVel;
                    StateMachine.SetState<FStateAir>();
                    return;
                }
        
                Rigidbody.linearVelocity = newDir;
                StateMachine.SetState<FStateGrind>().SetRail(_targetRail);
            }
        }

        public void Set(Vector3 start, Vector3 end, Rail targetRail, Vector3 savedVelocity, bool isLeft)
        {
            _start = start;
            _end = end;
            _targetRail = targetRail;
            _savedVelocity = savedVelocity;
            IsLeft = isLeft;
            _data = new SplineData(targetRail.Container, end);
        }
    }
}