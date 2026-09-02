using SurgeEngine.Source.Code.Infrastructure.Config.Sonic;

namespace SurgeEngine.Source.Code.Core.Character.System.Characters.Sonic
{
    public class SonicInput : CharacterInput
    {
        public bool DriftHeld => DriftHeldCheck();

        private bool DriftHeldCheck()
        {
            Character.TryGetConfig(out DriftConfig driftConfig);
            return BHeld || driftConfig.allowTriggerToActivate && TriggerInputAction.IsPressed();
        }
    }
}