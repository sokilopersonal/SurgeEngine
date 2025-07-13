using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
{
    public sealed class ActorFlags : ActorComponent
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
            flag.SetActor(Actor);
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
        protected ActorBase actor;
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
                    actor.Flags.RemoveFlag(type);
                }
            }
        }
        
        public void SetActor(ActorBase actor) => this.actor = actor;
    }

    public class AutorunFlag : Flag
    {
        private float _speed;
        private float _easeTime;
        private float _startSpeed;
        private float _elapsed;
        
        public AutorunFlag(FlagType type, bool isTemporary, float time, float speed, float easeTime, float startSpeed) : base(type,
            isTemporary, time)
        {
            this.type = FlagType.Autorun;
            
            _speed = speed;
            _easeTime = easeTime;
            _startSpeed = startSpeed;
        }

        public override void Count(float dt)
        {
            base.Count(dt);

            _elapsed += dt;
            float t = Mathf.Clamp01(_elapsed / _easeTime);
            float currentSpeed = Mathf.Lerp(_startSpeed, _speed, t);

            var k = actor.Kinematics;
            var rb = k.Rigidbody;
            var normal = k.Normal;
            Vector3 vertical = Vector3.Project(rb.linearVelocity, normal);
            rb.linearVelocity = actor.transform.forward * currentSpeed + vertical;
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
    }
}
