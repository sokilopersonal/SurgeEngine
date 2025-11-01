namespace SurgeEngine._Source.Code.Core.StateMachine.Base
{
    public abstract class FState
    {
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OnTick(float dt) { }
        public virtual void OnFixedTick(float dt) { }
    }
}