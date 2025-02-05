using SurgeEngine.Code.ActorHUD;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.Context
{
    [DefaultExecutionOrder(-9000)]
    public class SceneContext : MonoBehaviour
    {
        private static SceneContext _instance = null;
        
        [SerializeField] private Actor actor;
        [SerializeField] private ActorStageHUD actorHud;
        
        public static Actor Actor => _instance.actor;
        public static ActorStageHUD ActorHud => _instance.actorHud;

        private void Awake()
        {
            _instance = this;
            
            _ = new ActorContext(Actor);
            _ = new ActorHUDContext(ActorHud);
        }
    }
}