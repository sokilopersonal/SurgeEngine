using SurgeEngine.Code.Core.Actor.Sound;

namespace SurgeEngine.Code.Core.Actor.System
{
    public class ActorSounds : ActorComponent
    {
        internal override void Set(ActorBase actor)
        {
            base.Set(actor);
            
            foreach (ActorSound sound in GetComponents<ActorSound>())
            {
                sound.Initialize(actor);
            }
        }
    }
}