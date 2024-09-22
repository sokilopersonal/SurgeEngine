using System;
using System.Collections;
using System.Collections.Generic;
using SurgeEngine.Code.ActorSoundEffects;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorSounds : ActorComponent
    {
        [SerializeField] private List<SoundData> sounds;
        [SerializeField] private BoostAudioDistortion distortion;
        
        private float _lastBoostVoiceTime;
        
        private const float BOOST_VOICE_DELAY = 1.5f;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            
            actor.stateMachine.GetSubState<FBoost>().OnActiveChanged += OnBoostActivate;
            actor.stateMachine.OnStateAssign += OnStateAssign;
            ActorEvents.OnRingCollected += OnRingCollected;
            _lastBoostVoiceTime = Time.time - BOOST_VOICE_DELAY;
        }

        private void OnDisable()
        {
            actor.stateMachine.GetSubState<FBoost>().OnActiveChanged -= OnBoostActivate;
            ActorEvents.OnRingCollected -= OnRingCollected;
        }

        private void OnBoostActivate(FSubState arg1, bool arg2)
        {
            if (arg1 is FBoost && arg2)
            {
                if (_lastBoostVoiceTime + BOOST_VOICE_DELAY < Time.time)
                {
                    PlaySound($"Boost_Start{Random.Range(1, 4)}", false);
                    _lastBoostVoiceTime = Time.time;
                }
                
                PlaySound("BoostLoop", true);
                PlaySound("BoostJet", false);
                PlaySound("BoostForce", false);
                PlaySound("BoostImpulse", false);
                
                distortion.ToggleDistortion();
            }
            else if (arg1 is FBoost && !arg2)
            {
                StopSound("BoostLoop", true);
                StopSound("BoostJet", false);
                
                distortion.ToggleDistortion();
            }
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is FStateJump)
            {
                PlaySound($"Jump_Start{Random.Range(1, 4)}", false);
                
                PlaySound("Spin", false);
            }
        }

        private void OnRingCollected(Ring obj)
        {
            PlaySound("Ring", false);
            PlaySound($"Ring_Sparkle{Random.Range(1, 4)}", false);
        }

        private void PlaySound(string name, bool loop)
        {
            SoundData sound = sounds.Find(x => x.name == name);
            if (loop)
            {
                sound.StopCoroutine(this);
                
                sound.Play();
            }
            else sound.PlayOneShot();
        }

        private void StopSound(string name, bool loop)
        {
            SoundData sound = sounds.Find(x => x.name == name);
            
            if (loop) sound.StopFade(this, 0.3f);
            else sound.source.Stop();
        }
    }

    [Serializable]
    public class SoundData
    {
        public string name;
        public AudioSource source;
        public AudioClip clip;
        [Range(0, 1)] public float volume = 1;
        
        public Coroutine fadeCoroutine;

        public void Play()
        {
            source.clip = clip;
            source.volume = volume;
            source.Play();
        }
        
        public void PlayOneShot()
        {
            source.Stop();
            source.PlayOneShot(clip, volume);
        }

        public void StopFade(MonoBehaviour owner, float time)
        {
            fadeCoroutine = owner.StartCoroutine(FadeVolume(time));
        }
        
        public void StopCoroutine(MonoBehaviour owner)
        {
            if (fadeCoroutine != null) owner.StopCoroutine(fadeCoroutine);
        }

        private IEnumerator FadeVolume(float time)
        {
            float t = 0;
            float duration = time;

            while (t < duration)
            {
                source.volume = Mathf.Lerp(volume, 0, t / duration);
                t += Time.deltaTime;
                yield return null;
            }
            
            source.volume = 0;
            source.Stop();
        }
    }

    public class Sound
    {
        
    }
}