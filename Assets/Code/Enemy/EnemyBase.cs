using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Enemy.States;
using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.StateMachine
{
    [DefaultExecutionOrder(-9888)]
    public class EnemyBase : MonoBehaviour
    {
        public FStateMachine stateMachine;

        protected virtual void Awake()
        {
            stateMachine = new FStateMachine();
        }
        
        public bool CanBeDamaged()
        {
            var context = ActorContext.Context;
            var state = context.stateMachine.CurrentState;

            if (context.stateMachine.GetSubState<FBoost>().Active)
            {
                return true;
            }

            return state is FStateHoming or FStateDrift or FStateSliding or FStateJump || stateMachine.IsExact<EGStateDead>();
        }
    }
}