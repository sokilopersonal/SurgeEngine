using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;

namespace SurgeEngine.Code.Parameters
{
    public class FActorState : FState
    {
        protected Actor Actor { get; private set; }
        protected ActorInput Input { get; private set; }
        protected ActorStats Stats { get; private set; }
        protected ActorAnimation Animation { get; private set; }
        protected ActorKinematics Kinematics { get; private set; }
        protected FStateMachine StateMachine { get; private set; }

        public void SetOwner(Actor owner)
        {
            Actor = owner;
            
            StateMachine = owner.stateMachine;
            
            Input = owner.input;
            Stats = owner.stats;
            Animation = owner.animation;
            Kinematics = owner.kinematics;
        }
    }
}