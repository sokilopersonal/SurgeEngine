namespace SurgeEngine._Source.Code.Core.Character.System.Characters.Sonic
{
    public class SonicInput : CharacterInput
    {
        public bool DriftHeld => BHeld || TriggerAction.IsPressed();
    }
}