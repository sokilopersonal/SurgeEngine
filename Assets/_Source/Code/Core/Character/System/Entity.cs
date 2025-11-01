using SurgeEngine._Source.Code.Core.StateMachine;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.System
{
    public abstract class Entity : MonoBehaviour
    {
        public FStateMachine stateMachine;

        protected virtual void Awake()
        {
            stateMachine = new FStateMachine();
        }

        protected virtual void Update()
        {
            stateMachine?.Tick(Time.deltaTime);
        }

        protected virtual void FixedUpdate()
        {
            stateMachine?.FixedTick(Time.fixedDeltaTime);
        }

        protected virtual void LateUpdate()
        {
        }
    }
}