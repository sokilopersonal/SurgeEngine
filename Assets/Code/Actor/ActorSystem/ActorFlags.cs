using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorFlags : ActorComponent
    {
        public HashSet<Flag> hash;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            
            hash = new HashSet<Flag>();
        }

        public void AddFlag(Flag flag)
        {
            hash.Add(flag);
        }
        
        public void RemoveFlag(FlagType type)
        {
            hash.Remove(hash.FirstOrDefault(f => f.type == type));
        }
        
        public bool HasFlag(FlagType type)
        {
            return hash.Any(f => f.type == type);
        }

        public Flag GetFlag(FlagType type)
        {
            return hash.FirstOrDefault(f => f.type == type);
        }

        public bool CheckForTag(string tag)
        {
            return hash.Any(f => f.tags != null && f.tags.Contains(tag));
        }

        private void Update()
        {
            foreach (var flag in hash.ToList())
            {
                flag.Count(Time.deltaTime);
            }
        }
    }
    
    public class Flag
    {
        private HashSet<Flag> hs;
        public FlagType type;
        public string[] tags;
        private readonly bool isTemporary;
        private readonly float time;
        private float timer;
        
        public Flag(FlagType type, string[] tags, bool isTemporary = true, float time = 1) // We need HashSet link to remove this flag
        {
            hs = ActorContext.Context.flags.hash;
            
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
                    hs.Remove(this);
                }
            }
        }

        public float GetTime() => timer;
    }

    public enum FlagType
    {
        OutOfControl,
        DontClampVerticalSpeed
    }
}