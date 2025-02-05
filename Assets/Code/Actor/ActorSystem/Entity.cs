using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public abstract class Entity : MonoBehaviour
    {
        private readonly Dictionary<Type, ScriptableObject> _configs = new Dictionary<Type, ScriptableObject>();
        
        protected void AddConfig(ScriptableObject config)
        {
            _configs.Add(config.GetType(), config);
        }
        
        public void TryGetConfig<T>(out T request) where T : ScriptableObject
        {
            if (_configs.TryGetValue(typeof(T), out var result))
            {
                request = (T)result;
                return;
            }

            request = null;
        }
    }
}