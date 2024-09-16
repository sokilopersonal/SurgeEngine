using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurgeEngine.Code.StateMachine
{
    [Serializable]
    public class FStateMachine
    {
        public FState CurrentState { get; private set; }
        public string currentStateName;
        
        private Dictionary<Type, FState> _states = new Dictionary<Type, FState>();
        
        public void AddState(FState state)
        {
            _states.Add(state.GetType(), state);
        }
        
        public void SetState<TState>() where TState : FState
        {
            if (CurrentState != null)
            {
                CurrentState.OnExit();
            }
            
            CurrentState = _states[typeof(TState)];
            CurrentState.OnEnter();
            
            currentStateName = CurrentState.GetType().Name;
        }
        
        public TState GetState<TState>() where TState : FState
        {
            return _states[typeof(TState)] as TState;
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