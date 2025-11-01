using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.StateMachine.Base;

namespace SurgeEngine.Source.Code.Core.Character.System.Characters.Sonic
{
    public class SonicKinematics : CharacterKinematics
    {
        private FBoost _boost;

        protected override void Awake()
        {
            base.Awake();
            
            character.StateMachine.GetState(out _boost);
        }

        protected override bool CanReturnToBaseSpeed()
        {
            if (_boost != null)
            {
                var boostActive = _boost.Active;
            
                if (!boostActive)
                    return true;
            }
            
            return base.CanReturnToBaseSpeed();
        }

        protected override bool CanDecelerate()
        {
            return base.CanDecelerate() && _boost != null && !_boost.Active;
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