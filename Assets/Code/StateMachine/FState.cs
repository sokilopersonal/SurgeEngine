using UnityEngine;

namespace SurgeEngine.Code.StateMachine
{
    public abstract class FState : MonoBehaviour
    {
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OnTick(float dt) { }
        public virtual void OnFixedTick(float dt) { }
        public virtual void OnLateTick(float dt) { }
    }
}