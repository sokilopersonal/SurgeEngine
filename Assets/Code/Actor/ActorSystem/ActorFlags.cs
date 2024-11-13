using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorFlags : MonoBehaviour, IActorComponent
    {
        public Actor actor { get; set; }

        public List<Flag> list;
        public FlagType flagType;

        private void Awake()
        {
            list = new List<Flag>();
        }

        public void OnInit()
        {
        }

        public void AddFlag(Flag flag, bool overwrite = false)
        {
            var f = list.FirstOrDefault(f => f.type == flag.type);
            if (f != null)
            {
                if (!overwrite)
                {
                    return;
                }
            
                list.Remove(f);
                list.Add(flag);
            }
            else
            {
                list.Add(flag);
            }
        }

        public void RemoveFlag(FlagType type)
        {
            list.Remove(list.FirstOrDefault(f => f.type == type));
        }

        public bool HasFlag(FlagType type)
        {
            return list.Any(f => f.type == type);
        }

        public Flag GetFlag(FlagType type)
        {
            return list.FirstOrDefault(f => f.type == type);
        }

        public bool CheckForTag(string tag)
        {
            return list.Any(f => f.tags != null && f.tags.Contains(tag));
        }

        private void Update()
        {
            foreach (var flag in list.ToList())
            {
                flag.Count(Time.deltaTime);
            }
        }
    }
    
    [Serializable]
    public class Flag
    {
        private List<Flag> hs;
        public FlagType type;
        public string[] tags;
        [SerializeField] private bool isTemporary;
        [SerializeField] private float time;
        [SerializeField] private float timer;
        
        public Flag(FlagType type, string[] tags, bool isTemporary = true, float time = 1) // We need List link to remove this flag
        {
            hs = ActorContext.Context.flags.list;
            
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

    [Flags]
    public enum FlagType
    {
        None,
        OutOfControl,
        OnWater,
        Underwater,
    }
}