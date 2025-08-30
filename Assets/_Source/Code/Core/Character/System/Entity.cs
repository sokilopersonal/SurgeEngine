using SurgeEngine.Code.Core.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
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
            stateMachine?.LateTick(Time.deltaTime);
        }
    }
}