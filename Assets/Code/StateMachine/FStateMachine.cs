using System;
using System.Collections.Generic;

namespace SurgeEngine.Code.StateMachine
{
    [Serializable]
    public class FStateMachine
    {
        public FState CurrentState { get; private set; }
        public string currentStateName;
        
        private Dictionary<Type, FState> _states = new Dictionary<Type, FState>();
        private Dictionary<Type, FSubState> _subStates = new Dictionary<Type, FSubState>();
        
        public event Action<FState> OnStateEnter; 
        
        public void AddState(FState state)
        {
            _states.Add(state.GetType(), state);
        }
        
        public void AddSubState(FSubState subState)
        {
            _subStates.Add(subState.GetType(), subState);
        }
        
        public void SetState<T>() where T : FState
        {
            if (CurrentState != null)
            {
                CurrentState.OnExit();
            }
            
            CurrentState = _states[typeof(T)];
            CurrentState.OnEnter();
            OnStateEnter?.Invoke(CurrentState);
            
            currentStateName = CurrentState.GetType().Name;
        }
        
        public TState GetState<TState>() where TState : FState
        {
            return _states[typeof(TState)] as TState;
        }
        
        public T GetSubState<T>() where T : FSubState
        {
            return _subStates[typeof(T)] as T;
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
}