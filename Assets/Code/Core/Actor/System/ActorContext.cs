using Zenject;

namespace SurgeEngine.Code.Core.Actor.System
{
    public class ActorContext
    {
        private static ActorBase _actor;
        public static ActorBase Context => _actor;
        
        [Inject]
        private void SetActor(ActorBase actor)
        {
            _actor = actor;
        }
    }
}