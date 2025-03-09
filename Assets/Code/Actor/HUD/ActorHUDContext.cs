namespace SurgeEngine.Code.Actor.HUD
{
    public class ActorHUDContext
    {
        private static ActorStageHUD _hud;
        public static ActorStageHUD Context => _hud;
        
        public ActorHUDContext(ActorStageHUD hud)
        {
            _hud = hud;
        }
    }
}