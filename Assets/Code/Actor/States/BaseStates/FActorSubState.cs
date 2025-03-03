using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;

namespace SurgeEngine.Code.ActorStates.BaseStates
{
    public class FActorSubState : FSubState
    {
        protected Actor actor { get; private set; }

        public FActorSubState(Actor owner)
        {
            actor = ActorContext.Context;
        }
    }
}