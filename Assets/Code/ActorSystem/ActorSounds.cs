using System;
using System.Collections.Generic;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorSounds : ActorComponent
    {
        [SerializeField] private List<SoundData> sounds;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            
            actor.stats.boost.OnActiveChanged += OnBoostActivate;
        }

        private void OnDisable()
        {
            actor.stats.boost.OnActiveChanged -= OnBoostActivate;
        }

        private void OnBoostActivate(FSubState arg1, bool arg2)
        {
            if (arg1 is FBoost && arg2)
            {
                PlaySound("Boost", true);
            }
            else if (arg1 is FBoost && !arg2)
            {
                StopSound("Boost");
            }
        }

        private void PlaySound(string name, bool loop = false)
        {
            SoundData sound = sounds.Find(x => x.name == name);
            sound.PlayAllClips();
            
            if (loop)
            {
                sound.PlayLoop();
            }
        }

        private void StopSound(string name)
        {
            SoundData sound = sounds.Find(x => x.name == name);
            sound.source.Stop();
        }
    }

    [Serializable]
    public class SoundData
    {
        public string name;
        public AudioSource source;
        public AudioClip[] clips;

        public void PlayAllClips()
        {
            foreach (var clip in clips)
            {
                source.PlayOneShot(clip);
            }
        }

        public void PlayLoop()
        {
            if (source.clip == null)
            {
                Debug.LogWarning($"There is no clip set for sound {name}");
                return;
            }
            
            source.loop = true;
            source.Play();
        }
    }
}