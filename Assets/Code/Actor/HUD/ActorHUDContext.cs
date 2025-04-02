using Zenject;

namespace SurgeEngine.Code.Actor.HUD
{
    public class ActorHUDContext
    {
        private static ActorStageHUD _hud;
        public static ActorStageHUD Context => _hud;
        
        [Inject]
        private void SetHUD(ActorStageHUD hud) => _hud = hud;
    }
}