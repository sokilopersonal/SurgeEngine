using System;
using System.Collections.Generic;

namespace SurgeEngine.Code.StateMachine
{
    [Serializable]
    public class FStateMachine
    {
        public FState CurrentState { get; private set; }
        public FState PreviousState { get; private set; }
        public string currentStateName;
        
        private Dictionary<Type, FState> _states = new Dictionary<Type, FState>();
        private Dictionary<Type, FSubState> _subStates = new Dictionary<Type, FSubState>();
        private List<FSubState> _subStatesList = new List<FSubState>();
        
        public event Action<FState> OnStateAssign;
        
        private float _inactiveDelay = 0f;
        
        public void AddState(FState state)
        {
            _states.Add(state.GetType(), state);
        }
        
        public void AddSubState(FSubState subState)
        {
            _subStates.Add(subState.GetType(), subState);
            _subStatesList.Add(subState);
        }
        
        public T SetState<T>(float inactiveDelay = 0, bool ignoreInactiveDelay = false, bool allowSameState = false) where T : FState
        {
            if (_inactiveDelay > 0f && !ignoreInactiveDelay) return null;

            var type = typeof(T);

            if (CurrentState != null && CurrentState.GetType() == type && !allowSameState)
            {
                return null;
            }

            if (_states.TryGetValue(type, out var newState))
            {
                CurrentState?.OnExit();
                PreviousState = CurrentState;
                CurrentState = newState;
                OnStateAssign?.Invoke(CurrentState);
                CurrentState.OnEnter();
                
                currentStateName = CurrentState.GetType().Name;
                
                _inactiveDelay = inactiveDelay;
                
                return CurrentState as T;
            }
            
            return null;
        }
        
        public TState GetState<TState>() where TState : FState
        {
            return _states[typeof(TState)] as TState;
        }
        
        public bool IsExact<T>() where T : FState
        {
            var type = typeof(T);
            return CurrentState != null && CurrentState.GetType() == type;
        }

        public bool IsPrevExact<T>() where T : FState
        {
            var type = typeof(T);
            return PreviousState != null && PreviousState.GetType() == type;
        }

        public bool IsPreviousState<TState>() where TState : FState
        {
            return PreviousState is TState;
        }
        
        public T GetSubState<T>() where T : FSubState
        {
            return _subStates[typeof(T)] as T;
        }

        public virtual void Tick(float dt)
        {
            if (_inactiveDelay > 0f)
            {
                _inactiveDelay -= dt;
            }
            
            CurrentState?.OnTick(dt);

            foreach (var subState in _subStatesList)
            {
                subState?.OnTick(dt);
            }
        }
        
        public void FixedTick(float dt)
        {
            CurrentState?.OnFixedTick(dt);
            
            foreach (var subState in _subStatesList)
            {
                subState?.OnFixedTick(dt);
            }
        }
        
        public virtual void LateTick(float dt)
        {
            CurrentState?.OnLateTick(dt);
            
            foreach (var subState in _subStatesList)
            {
                subState?.OnLateTick(dt);
            }
        }
    }
}