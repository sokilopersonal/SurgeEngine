namespace SurgeEngine.Code.ActorSystem
{
    public class ActorContext
    {
        private static Actor _actor;
        public static Actor Context => _actor;
        
        public ActorContext(Actor actor)
        {
            _actor = actor;
        }
    }
}