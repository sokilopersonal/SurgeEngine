using System;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using SurgeEngine.Source.Code.Core.Character.States;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class CharacterFlags : CharacterComponent
    {
        [SerializeField, ReadOnly] private FlagType flags;

        private Dictionary<FlagType, Flag> _runtimeFlags;

        private void Awake()
        {
            _runtimeFlags = new Dictionary<FlagType, Flag>();
        }

        private void Update()
        {
            foreach (var pair in _runtimeFlags.ToArray())
            {
                var flag = pair.Value;
                flag.Update();

                if (flag.IsFinished)
                    RemoveFlag(pair.Key);
            }
        }
        
        public void AddFlag(FlagType type)
        {
            if (type == FlagType.None) return;
            flags |= type;
        }
        
        public void AddFlag(Flag flag)
        {
            _runtimeFlags.Remove(flag.Type, out _);

            flag.Character = Character;

            _runtimeFlags[flag.Type] = flag;
            flags |= flag.Type;
        }

        public void RemoveFlag(FlagType type)
        {
            _runtimeFlags.Remove(type, out var flag);

            flags &= ~type;
        }

        public bool GetFlag<T>(out T flag) where T : Flag
        {
            foreach (var f in _runtimeFlags.Values)
            {
                if (f is T typed)
                {
                    flag = typed;
                    return true;
                }
            }

            flag = null;
            return false;
        }

        public void Clear()
        {
            _runtimeFlags.Clear();
            flags = FlagType.None;
        }

        public bool HasFlag(FlagType type)
        {
            return (flags & type) != 0;
        }
    }

    public class Flag
    {
        public CharacterBase Character { get; set; }

        public FlagType Type { get; }
        public bool IsFinished => _isTemporary && _timer >= _time;

        private readonly bool _isTemporary;
        private readonly float _time;
        private float _timer;

        public Flag(FlagType type)
        {
            if (type == FlagType.None)
                throw new ArgumentException("Flag type cannot be None.");

            Type = type;
        }

        public Flag(FlagType type, float time)
        {
            if (type == FlagType.None)
                throw new ArgumentException("Flag type cannot be None.");

            Type = type;
            _isTemporary = time > 0;
            _time = time;
        }

        public virtual void Update()
        {
            if (_isTemporary)
                _timer += Time.deltaTime;
        }
    }

    public class AutorunFlag : Flag
    {
        private readonly float _targetSpeed;
        private readonly float _easeTime;
        private float _currentEaseTime;
        private float _initialSpeed;
        private bool _accelerationStarted;

        public float PathEaseTime { get; }

        public AutorunFlag(float time, float speed, float easeTime, float pathEaseTime = 0)
            : base(FlagType.Autorun, time)
        {
            _targetSpeed = speed;
            _easeTime = easeTime;
            PathEaseTime = pathEaseTime;
        }

        public override void Update()
        {
            base.Update();

            var kinematics = Character.Kinematics;

            if (!_accelerationStarted)
            {
                _initialSpeed = kinematics.Speed;
                _accelerationStarted = true;
            }

            _currentEaseTime += Time.deltaTime;

            float progress = Mathf.Clamp01(_currentEaseTime / _easeTime);
            float currentTargetSpeed = Mathf.Lerp(_initialSpeed, _targetSpeed, progress);

            if (kinematics.Speed < currentTargetSpeed || _targetSpeed == 0)
            {
                Vector3 currentVelocity = kinematics.Velocity;
                Vector3 planarVelocity = Vector3.ProjectOnPlane(currentVelocity, kinematics.Normal);
                Vector3 verticalVelocity = currentVelocity - planarVelocity;

                Vector3 newPlanarVelocity = Character.transform.forward * currentTargetSpeed;

                kinematics.Rigidbody.linearVelocity = newPlanarVelocity + verticalVelocity;

                if (_targetSpeed == 0 && _currentEaseTime >= _easeTime)
                {
                    if (kinematics.CheckForGround(out _))
                    {
                        Character.StateMachine.SetState<FStateIdle>();
                    }
                }
            }
        }
    }

    public class SlowdownFlag : Flag
    {
        private readonly float _maxSpeed;

        public SlowdownFlag(float time, float maxSpeed)
            : base(FlagType.Slowdown, time)
        {
            _maxSpeed = maxSpeed;
        }

        public override void Update()
        {
            base.Update();

            var kinematics = Character.Kinematics;
            if (kinematics.CheckForGround(out _))
            {
                kinematics.ClampVelocityToMax(_maxSpeed);
            }
        }
    }

    [Flags]
    public enum FlagType
    {
        None = 0,
        OutOfControl = 1,
        OnWater = 2,
        Invincible = 4,
        Autorun = 8,
        Slowdown = 16,
        Skydiving = 32
    }
}