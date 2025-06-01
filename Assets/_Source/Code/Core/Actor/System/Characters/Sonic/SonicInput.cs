namespace SurgeEngine.Code.Core.Actor.System.Characters.Sonic
{
    public class SonicInput : ActorInput
    {
        public bool DriftHeld => BHeld || TriggerAction.IsPressed();
    }
}