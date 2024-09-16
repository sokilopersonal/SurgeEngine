namespace SurgeEngine.Code.StateMachine
{
    public class FStateMachine
    {
        public FState CurrentState { get; private set; }
        
        public void SetState(FState state)
        {
            if (CurrentState != null)
                CurrentState.OnExit();
            
            CurrentState = state;
            CurrentState.OnEnter();
        }

        public void Tick(float dt)
        {
            CurrentState?.OnTick(dt);
        }
        
        public void FixedTick(float dt)
        {
            CurrentState?.OnFixedTick(dt);
        }
        
        public void LateTick(float dt)
        {
            CurrentState?.OnLateTick(dt);
        }
    }

	public class FState : FStateMachine
	{
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OnTick(float dt) { }
        public virtual void OnFixedTick(float dt) { }
        public virtual void OnLateTick(float dt) { }
    }
}