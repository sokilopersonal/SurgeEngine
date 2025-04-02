using System.Collections;
using FMODUnity;
using UnityEngine;

namespace SurgeEngine.Code.Actor.Sound
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
                RuntimeManager.StudioSystem.setParameterByName("Distort", 1);
            }
            else
            {
                RuntimeManager.StudioSystem.setParameterByName("Distort", 0);
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