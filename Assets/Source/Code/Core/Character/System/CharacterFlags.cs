using System;
using System.Collections.Generic;
using System.Linq;
using Alchemy.Inspector;
using SurgeEngine.Source.Code.Core.Character.States;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class CharacterFlags : CharacterComponent
    {
        [SerializeField, ReadOnly] private FlagType flags;
        private HashSet<Flag> _list;
        
        [Inject] private DiContainer _diContainer;

        private void Awake()
        {
            _list = new HashSet<Flag>();
        }

        private void Update()
        {
            foreach (var flag in _list.ToList())
            {
                flag.Update();
                if (flag.IsFinished)
                    RemoveFlag(flag.Type);
            }
        }

        public void AddFlag(Flag flag)
        {
            Flag existingFlag = _list.FirstOrDefault(f => f.Type == flag.Type);
            if (existingFlag != null)
            {
                _list.Remove(existingFlag);
            }
            flags |= flag.Type;
            _list.Add(flag);
            _diContainer.Inject(flag);
        }

        public void RemoveFlag(FlagType type)
        {
            flags &= ~type;
            Flag flagToRemove = _list.FirstOrDefault(f => f.Type == type);
            if (flagToRemove != null)
            {
                _list.Remove(flagToRemove);
            }
        }
        
        public bool GetFlag<T>(out T flag) where T : Flag
        {
            flag = _list.OfType<T>().FirstOrDefault();
            return flag != null;
        }

        public void Clear()
        {
            _list.Clear();
            flags = FlagType.None;
        }

        public bool HasFlag(FlagType type)
        {
            return flags.HasFlag(type);
        }
    }
    
    public class Flag
    {
        [Inject] protected CharacterBase Character { get; }
        
        public FlagType Type { get; protected set; }
        public bool IsFinished => _isTemporary && _timer >= _time;
        private readonly bool _isTemporary;
        private readonly float _time;
        private float _timer;

        public Flag(FlagType type)
        {
            Type = type;
            if (type == FlagType.None)
                throw new ArgumentException("Flag type cannot be None.");
        }
        
        public Flag(FlagType type, float time)
        {
            Type = type;
            _isTemporary = time > 0;
            _time = time;
            
            if (type == FlagType.None)
                throw new ArgumentException("Flag type cannot be None.");
        }

        /// <summary>
        /// Update flag logic.
        /// </summary>
        /// <returns>true when time finished.</returns>
        public virtual void Update()
        {
            if (_isTemporary)
            {
                _timer += Time.deltaTime;
            }
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

        public AutorunFlag(FlagType type, float time, float speed, float easeTime, float pathEaseTime = 0) : base(type, time)
        {
            Type = FlagType.Autorun;
            
            _targetSpeed = speed;
            _easeTime = easeTime;
            _currentEaseTime = 0f;
            _accelerationStarted = false;
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

                if (_targetSpeed == 0)
                {
                    if (_currentEaseTime >= _easeTime)
                    {
                        if (kinematics.CheckForGround(out _))
                        {
                            Character.StateMachine.SetState<FStateIdle>();
                        }
                    }
                }
            }
        }
    }

    public class SlowdownFlag : Flag
    {
        private readonly float _maxSpeed;

        public SlowdownFlag(FlagType type, float time, float maxSpeed) : base(type, time)
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
