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
        
        public void RemoveFlag(Flag flag)
        {
            hash.Remove(flag);
        }
        
        public bool HasFlag(FlagType type)
        {
            return hash.Any(f => f.type == type);
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
        private readonly bool isTemporary;
        private readonly float time;
        private float timer;
        
        public Flag(FlagType type, bool isTemporary = true, float time = 1) // We need HashSet link to remove this flag
        {
            hs = ActorContext.Context.flags.hash;
            
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
                    hs.Remove(this);
                }
            }
        }
    }

    public enum FlagType
    {
        OutOfControl
    }
}