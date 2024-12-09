using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.States
{
    public class EGState : FEState
    {
        protected EggFighter eggFighter;
        
        public EGState(EggFighter eggFighter, Transform transform, Rigidbody rb) : base(transform, rb)
        {
            this.eggFighter = eggFighter;
        }
    }
}