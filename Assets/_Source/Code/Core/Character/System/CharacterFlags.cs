using System;
using System.Collections.Generic;
using System.Linq;
using SurgeEngine.Code.Core.Actor.States;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
{
    public sealed class CharacterFlags : CharacterComponent
    {
        private HashSet<Flag> list;
        public FlagType flagType;

        private void Awake()
        {
            list = new HashSet<Flag>();
        }

        private void Update()
        {
            foreach (var flag in list.ToList())
            {
                flag.Count(Time.deltaTime);
            }
        }

        public void AddFlag(Flag flag)
        {
            Flag existingFlag = list.FirstOrDefault(f => f.type == flag.type);
            if (existingFlag != null)
            {
                list.Remove(existingFlag);
            }
            flagType |= flag.type;
            flag.SetActor(character);
            list.Add(flag);
        }

        public void RemoveFlag(FlagType type)
        {
            flagType &= ~type;
            Flag flagToRemove = list.FirstOrDefault(f => f.type == type);
            if (flagToRemove != null)
            {
                list.Remove(flagToRemove);
            }
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
        private bool _accelerationStarted;
        
        public AutorunFlag(FlagType type, bool isTemporary, float time, float speed, float easeTime) : base(type,
            isTemporary, time)
        {
            this.type = FlagType.Autorun;
            
            _targetSpeed = speed;
            _easeTime = easeTime;
            _currentEaseTime = 0f;
            _accelerationStarted = false;
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
        Underwater = 4,
        Invincible = 8,
        Autorun = 16,
        Slowdown = 32
    }
}
