using System.Collections.Generic;
using System.Linq;
using SurgeEngine.Code.Actor.Sound;

namespace SurgeEngine.Code.Actor.System
{
    public class ActorSounds : ActorComponent
    {
        private List<ActorSound> _sounds = new List<ActorSound>();

        internal override void Set(ActorBase actor)
        {
            base.Set(actor);
            
            _sounds = GetComponents<ActorSound>().ToList();

            foreach (ActorSound sound in _sounds)
            {
                sound.Initialize(actor);
            }
        }
    }
}