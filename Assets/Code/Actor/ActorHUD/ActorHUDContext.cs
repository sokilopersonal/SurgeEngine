using SurgeEngine.Code.ActorHUD;
using UnityEngine;

namespace SurgeEngine.Code.ActorHUD
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