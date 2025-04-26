using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.Actor.Sound
{
    public class VoiceHandler : MonoBehaviour
    {
        private float _voiceDelayTimer;
        private bool _canPlayVoice => _voiceDelayTimer <= 0;
        private const float VOICE_DELAY = 1.5f;
        
        private void Update()
        {
            if (_voiceDelayTimer > 0)
            {
                _voiceDelayTimer -= Time.deltaTime;
            }
            else
            {
                _voiceDelayTimer = 0;
            }
        }

        public void Play(EventReference voice)
        {
            if (_canPlayVoice)
            {
                RuntimeManager.PlayOneShot(voice);
                
                _voiceDelayTimer = VOICE_DELAY;
            }
        }

        public void Play(EventInstance voice)
        {
            if (_canPlayVoice)
            {
                voice.stop(STOP_MODE.IMMEDIATE);
                voice.start();
                
                _voiceDelayTimer = VOICE_DELAY;
            }
        }

        public void Stop(EventInstance voice)
        {
            voice.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
}