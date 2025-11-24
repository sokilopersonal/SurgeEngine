using System;
using System.Collections.Generic;
using System.Linq;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Core.StateMachine.Interfaces;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.StateMachine
{
    public class FStateMachine
    {
        public FState CurrentState { get; private set; }
        public FState PreviousState { get; private set; }

        protected readonly Dictionary<Type, FState> states = new();
        private readonly List<FState> _statesList = new();
        private readonly Dictionary<Type, FSubState> _subStates = new();
        private readonly List<FSubState> _subStatesList = new();
        private readonly List<IStateTimeout> _stateTimeouts = new();
        
        /// <summary>
        /// Called after the previous state ends and before the next state is set.
        /// </summary>
        public event Action<FState> OnStateEarlyAssign;
        
        /// <summary>
        /// Called after the previous state ends and after the next state is set.
        /// </summary>
        public event Action<FState> OnStateAssign;
        
        public void AddState(FState state)
        {
            states.Add(state.GetType(), state);
            _statesList.Add(state);

            if (state is IStateTimeout timeout)
            {
                _stateTimeouts.Add(timeout);
            }
        }
        
        public void AddSubState(FSubState subState)
        {
            _subStates.Add(subState.GetType(), subState);
            _subStatesList.Add(subState);
        }

        public T SetState<T>(bool allowSameState = false) where T : FState
        {
            Type type = typeof(T);

            if (CurrentState != null && CurrentState.GetType() == type && !allowSameState)
            {
                return null;
            }

            if (states.TryGetValue(type, out FState newState))
            {
                if (newState is IStateTimeout timeout)
                {
                    if (Mathf.Approximately(timeout.Timeout, 0f))
                    {
                        EnterState(newState);
                    }
                }
                else
                {
                    EnterState(newState);
                }
 
                return CurrentState as T;
            }
            
            return null;
        }

        protected virtual void EnterState(FState newState)
        {
            CurrentState?.OnExit();
            PreviousState = CurrentState;
            OnStateEarlyAssign?.Invoke(newState);
            CurrentState = newState;
            OnStateAssign?.Invoke(CurrentState);
            CurrentState?.OnEnter();
        }

        public T GetState<T>() where T : FState
        {
            Type type = typeof(T);
            return states[type] as T;
        }

        public bool GetState<T>(out T state) where T : FState
        {
            Type type = typeof(T);
            if (states.TryGetValue(type, out var st))
            {
                state = st as T;
            }
            else
            {
                if (_subStates.TryGetValue(type, out var subState))
                {
                    state = subState as T;
                }
                else
                {
                    state = null;
                }
            }
            
            return state != null;
        }
        
        public T[] GetAllStatesOfType<T>() where T : FState
        {
            return states.Values
                .Where(state => typeof(T).IsAssignableFrom(state.GetType()))
                .Select(state => state as T)
                .ToArray();
        }
        
        public bool IsExact<T>() where T : FState
        {
            Type type = typeof(T);
            return CurrentState != null && CurrentState.GetType() == type;
        }

        public bool IsPrevExact<T>() where T : FState
        {
            Type type = typeof(T);
            return PreviousState != null && PreviousState.GetType() == type;
        }

        public virtual void Tick(float dt)
        {
            CurrentState?.OnTick(dt);

            foreach (FSubState subState in _subStatesList)
            {
                subState?.OnTick(dt);
            }
            
            foreach (IStateTimeout timeout in _stateTimeouts)
            {
                timeout?.Tick(dt);
            }
        }
        
        public void FixedTick(float dt)
        {
            CurrentState?.OnFixedTick(dt);
            
            foreach (FSubState subState in _subStatesList)
            {
                subState?.OnFixedTick(dt);
            }
        }
    }
}