using System;

namespace SurgeEngine.Code.StateMachine
{
    public abstract class FSubState : FState
    {
        private bool active;

        public bool Active
        {
            get { return active; }
            set
            {
                if (active != value) OnActiveChanged?.Invoke(this, value);
                active = value;
            }
        }
        
        public event Action<FSubState, bool> OnActiveChanged; 
    }
}