using Zenject;

namespace SurgeEngine.Code.Core.Actor.System
{
    public class ActorContext
    {
        private static ActorBase _actor;
        public static ActorBase Context => _actor;
        
        public static void Set(ActorBase actor)
        {
            _actor = actor;
        }
    }
}