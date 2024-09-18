using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;

namespace SurgeEngine.Code.Parameters
{
    public class FActorSubState : FSubState
    {
        protected Actor actor { get; private set; }

        public void SetOwner(Actor owner)
        {
            actor = owner;
        }
    }
}