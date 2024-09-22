using System.Collections;
using FMODUnity;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class BoostAudioDistortion : MonoBehaviour
    {
        private bool _enabled;
        
        private Coroutine _fadeCoroutine;
        
        public void Toggle()
        {
            _enabled = !_enabled;

            if (_enabled)
            {
                if (_fadeCoroutine != null)
                    StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = StartCoroutine(FadeDistort(1f, 0.2f));
            }
            else
            {
                if (_fadeCoroutine != null)
                    StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = StartCoroutine(FadeDistort(0f, 0.2f));
            }
        }

        private IEnumerator FadeDistort(float value, float duration)
        {
            float t = 0;

            while (t < duration)
            {
                RuntimeManager.StudioSystem.setParameterByName("Distort", value);
                t += Time.deltaTime;
                yield return null;
            }
            
            RuntimeManager.StudioSystem.setParameterByName("Distort", value);
        }
    }
}