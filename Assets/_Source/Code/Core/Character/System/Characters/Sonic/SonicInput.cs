namespace SurgeEngine.Code.Core.Actor.System.Characters.Sonic
{
    public class SonicInput : CharacterInput
    {
        public bool DriftHeld => BHeld || TriggerAction.IsPressed();
    }
}