using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.StateMachine.Base;

namespace SurgeEngine.Code.Core.Actor.System.Characters.Sonic
{
    public class SonicKinematics : ActorKinematics
    {
        protected override bool CanReturnToBaseSpeed()
        {
            return !Actor.StateMachine.GetSubState<FBoost>().Active;
        }

        protected override bool CanDecelerate()
        {
            return !Actor.StateMachine.GetSubState<FBoost>().Active && base.CanDecelerate();
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