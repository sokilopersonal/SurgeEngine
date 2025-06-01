using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Infrastructure.Tools;

namespace SurgeEngine.Code.Core.Actor.System.Characters.Sonic
{
    public class SonicKinematics : ActorKinematics
    {
        public HomingTarget HomingTarget { get; private set; }
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            FindHomingTarget();
        }

        private void FindHomingTarget()
        {
            if (Actor.StateMachine.CurrentState is FStateAir)
            {
                HomingTarget = SonicTools.FindHomingTarget();
            }
            else
            {
                HomingTarget = null;
            }
        }
        
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