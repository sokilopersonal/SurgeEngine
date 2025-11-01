using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Source.Code.Core.Character.Sound
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

        public void Play(EventReference voice, bool ignoreDelay = false)
        {
            if (_canPlayVoice || ignoreDelay)
            {
                RuntimeManager.PlayOneShot(voice, gameObject.transform.position);
                
                _voiceDelayTimer = VOICE_DELAY;
            }
        }

        public void Play(EventInstance voice, bool ignoreDelay = false)
        {
            if (_canPlayVoice || ignoreDelay)
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