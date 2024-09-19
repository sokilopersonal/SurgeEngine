using UnityEngine;
using UnityEngine.Audio;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class BoostAudioDistortion : MonoBehaviour
    {
        [SerializeField] private AudioMixerSnapshot normalSnapshot;
        [SerializeField] private AudioMixerSnapshot distortedSnapshot;

        private bool _enabled;
        
        public void ToggleDistortion()
        {
            _enabled = !_enabled;

            if (_enabled)
            {
                distortedSnapshot.TransitionTo(1f);
            }
            else
            {
                normalSnapshot.TransitionTo(0.5f);
            }
        }
    }
}