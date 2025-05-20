using SurgeEngine.Code.Core.Actor.States.SonicSpecific;
using SurgeEngine.Code.Core.Actor.States.SonicSubStates;
using SurgeEngine.Code.Core.StateMachine.Base;

namespace SurgeEngine.Code.Core.Actor.System.Actors
{
    public class SonicKinematics : ActorKinematics
    {
        protected override bool CanReturnToBaseSpeed()
        {
            return !Actor.stateMachine.GetSubState<FBoost>().Active;
        }

        protected override bool CanDecelerate()
        {
            return !Actor.stateMachine.GetSubState<FBoost>().Active && base.CanDecelerate();
        }

        protected override void SetStateOnZeroSpeed(FState state)
        {
            switch (state)
            {
                case FStateCrawl:
                    return;
                case FStateSweepKick:
                    return;
                case FStateSit:
                    return;
            }
            
            base.SetStateOnZeroSpeed(state);
        }
    }
}