using System;

namespace SurgeEngine.Source.Code.Core.StateMachine.Base
{
    public abstract class FSubState : FState
    {
        private bool _active;

        public bool Active
        {
            get => _active;
            set
            {
                if (_active != value) OnActiveChanged?.Invoke(this, value);
                _active = value;
            }
        }
        
        public event Action<FSubState, bool> OnActiveChanged; 
    }
}