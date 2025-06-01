namespace SurgeEngine.Code.Core.Actor.System.Characters.Sonic
{
    public class SonicInput : ActorInput
    {
        public bool BoostPressed => LeftPressed;
        
        public bool DriftHeld => DownHeld || TriggerAction.IsPressed();
    }
}