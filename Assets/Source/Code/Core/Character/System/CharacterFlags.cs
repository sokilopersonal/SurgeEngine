using System;
using System.Collections.Generic;
using System.Linq;
using SurgeEngine.Source.Code.Core.Character.States;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public sealed class CharacterFlags : CharacterComponent
    {
        private HashSet<Flag> _list;
        public FlagType flagType;

        private void Awake()
        {
            _list = new HashSet<Flag>();
        }

        private void Update()
        {
            foreach (var flag in _list.ToList())
            {
                flag.Count(Time.deltaTime);
            }
        }

        public void AddFlag(Flag flag)
        {
            Flag existingFlag = _list.FirstOrDefault(f => f.type == flag.type);
            if (existingFlag != null)
            {
                _list.Remove(existingFlag);
            }
            flagType |= flag.type;
            flag.SetActor(Character);
            _list.Add(flag);
        }

        public void RemoveFlag(FlagType type)
        {
            flagType &= ~type;
            Flag flagToRemove = _list.FirstOrDefault(f => f.type == type);
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
            flagType = FlagType.None;
        }

        public bool HasFlag(FlagType type)
        {
            return flagType.HasFlag(type);
        }
    }
    
    public class Flag
    {
        public FlagType type;
        protected CharacterBase Character;
        protected readonly bool isTemporary;
        protected readonly float time;
        protected float timer;

        public Flag(FlagType type, bool isTemporary = true, float time = 1)
        {
            this.type = type;
            this.isTemporary = isTemporary;
            this.time = time;
        }

        public virtual void Count(float dt)
        {
            if (isTemporary)
            {
                timer += dt;
                if (timer >= time)
                {
                    Character.Flags.RemoveFlag(type);
                }
            }
        }
        
        public void SetActor(CharacterBase character) => this.Character = character;
    }

    public class AutorunFlag : Flag
    {
        private float _targetSpeed;
        private float _easeTime;
        private float _currentEaseTime;
        private float _initialSpeed;
        private float _pathEaseTime;
        private bool _accelerationStarted;

        public float PathEaseTime => _pathEaseTime;
        
        public AutorunFlag(FlagType type, bool isTemporary, float time, float speed, float easeTime, float pathEaseTime = 0) : base(type,
            isTemporary, time)
        {
            this.type = FlagType.Autorun;
            
            _targetSpeed = speed;
            _easeTime = easeTime;
            _currentEaseTime = 0f;
            _accelerationStarted = false;
            _pathEaseTime = pathEaseTime;
        }

        public override void Count(float dt)
        {
            base.Count(dt);

            var kinematics = Character.Kinematics;

            if (!_accelerationStarted)
            {
                _initialSpeed = kinematics.Speed;
                _accelerationStarted = true;
            }

            _currentEaseTime += dt;

            float progress = Mathf.Clamp01(_currentEaseTime / _easeTime);
            float currentTargetSpeed = Mathf.Lerp(_initialSpeed, _targetSpeed, progress);
            if (kinematics.Speed < currentTargetSpeed || _targetSpeed == 0)
            {
                Vector3 currentVelocity = kinematics.Velocity;
                Vector3 planarVelocity = Vector3.ProjectOnPlane(currentVelocity, kinematics.Normal);
                Vector3 verticalVelocity = currentVelocity - planarVelocity;

                Vector3 newPlanarVelocity = Character.transform.forward * currentTargetSpeed;

                kinematics.Rigidbody.linearVelocity = newPlanarVelocity + verticalVelocity;

                var path = kinematics.Path2D;
                if (path != null)
                {
                    path.Spline.EvaluateWorld(out _, out var tg, out var up, out var right);
                    
                    kinematics.Project(right);
                }

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

        public SlowdownFlag(FlagType type, bool isTemporary, float time, float maxSpeed) : base(type,
            isTemporary, time)
        {
            _maxSpeed = maxSpeed;
        }

        public override void Count(float dt)
        {
            base.Count(dt);
            
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
