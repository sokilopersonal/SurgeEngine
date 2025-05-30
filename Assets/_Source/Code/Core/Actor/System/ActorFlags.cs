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
            flag.actorFlags = this;
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
        public readonly FlagType type;
        public ActorFlags actorFlags;
        private readonly bool isTemporary;
        private readonly float time;
        private float timer;
        public string[] tags;

        public Flag(FlagType type, string[] tags, bool isTemporary = true, float time = 1)
        {
            this.type = type;
            this.tags = tags;
            this.isTemporary = isTemporary;
            this.time = time;
        }

        public void Count(float dt)
        {
            if (isTemporary)
            {
                timer += dt;
                if (timer >= time)
                {
                    actorFlags.RemoveFlag(type);
                }
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
        Invincible = 8
    }
}
