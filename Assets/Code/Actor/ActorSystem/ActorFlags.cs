using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorFlags : MonoBehaviour
    {
        public Actor actor { get; set; }

        private HashSet<Flag> list;
        public FlagType flagType;

        private void Awake()
        {
            list = new HashSet<Flag>();
        }

        public void OnInit()
        {
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
            var existingFlag = list.FirstOrDefault(f => f.type == flag.type);
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
            var flagToRemove = list.FirstOrDefault(f => f.type == type);
            if (flagToRemove != null)
            {
                list.Remove(flagToRemove);
            }
        }

        public bool HasFlag(FlagType type)
        {
            return flagType.HasFlag(type);
        }
        
        public Flag GetFlag(FlagType type) => list.FirstOrDefault(f => f.type == type);

        public bool CheckForTag(string tag)
        {
            return list.Any(f => f.tags != null && f.tags.Contains(tag));
        }
    }
    
    public class Flag
    {
        public FlagType type;
        public ActorFlags actorFlags;
        [SerializeField] private bool isTemporary;
        [SerializeField] private float time;
        [SerializeField] private float timer;
        public string[] tags;

        public Flag(FlagType type, string[] tags, bool isTemporary = true, float time = 1)
        {
            this.type = type;
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

        public float GetTime() => timer;
    }

    [Flags]
    public enum FlagType
    {
        None = 0,
        OutOfControl = 1,
        OnWater = 2,
        Underwater = 4
    }
}
