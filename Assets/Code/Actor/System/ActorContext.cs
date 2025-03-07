using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorContext
    {
        private static Actor _actor;
        public static Actor Context => _actor;
        
        [Inject]
        private void SetActor(Actor actor)
        {
            _actor = actor;
        }
    }
}