using System.Collections.Generic;
using System.Linq;
using SurgeEngine.Code.ActorSoundEffects;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorSounds : ActorComponent
    {
        private List<ActorSound> _sounds = new List<ActorSound>(); 
        
        private const float BOOST_VOICE_DELAY = 1.5f;

        private void Awake()
        {
            _sounds = GetComponents<ActorSound>().ToList();

            foreach (ActorSound sound in _sounds)
            {
                sound.Initialize();
            }
        }
        
        public void OnInit() { }
    }
}