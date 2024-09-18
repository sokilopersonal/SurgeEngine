using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;

namespace SurgeEngine.Code.Parameters
{
    public class FActorState : FState
    {
        protected Actor actor { get; private set; }
        protected ActorInput input { get; private set; }
        protected ActorStats stats { get; private set; }
        protected ActorAnimation animation { get; private set; }
        protected FStateMachine stateMachine { get; private set; }

        public void SetOwner(Actor owner)
        {
            actor = owner;
            
            stateMachine = owner.stateMachine;
            
            input = owner.input;
            stats = owner.stats;
            animation = owner.animation;
        }
    }
}