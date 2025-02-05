using System;
using System.Collections.Generic;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public abstract class Entity : MonoBehaviour
    {
        public FStateMachine stateMachine;
        private readonly Dictionary<Type, ScriptableObject> _configs = new Dictionary<Type, ScriptableObject>();

        protected virtual void Awake()
        {
            stateMachine = new FStateMachine();
        }

        private void Update()
        {
            stateMachine?.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            stateMachine?.FixedTick(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            stateMachine?.LateTick(Time.deltaTime);
        }
        
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