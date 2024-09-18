using System.Collections;
using UnityEngine;

namespace SurgeEngine.Code.ActorEffects
{
    public class BoostDistortion : MonoBehaviour
    {
        [SerializeField] private Material customPass;
        [SerializeField] private float duration;

        private Coroutine coroutine;
        private static readonly int WaveTime = Shader.PropertyToID("_WaveTime");
        private static readonly int SpawnPosition = Shader.PropertyToID("_SpawnPosition");

        public void Play(Vector3 viewportPosition)
        {
            customPass.SetFloat(WaveTime, -0.2f);
            customPass.SetVector(SpawnPosition, viewportPosition);

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            
            coroutine = StartCoroutine(PlayDistortion());
        }

        IEnumerator PlayDistortion()
        {
            float t = 0;
            float dur = duration;
            
            while (t < dur)
            {
                float value = Mathf.Lerp(-0.2f, 1.2f, EaseOutQuad(t / dur));
                customPass.SetFloat(WaveTime, value);
                t += Time.deltaTime;
                
                yield return null;
            }
        }

        private void OnDestroy()
        {
            customPass.SetFloat(WaveTime, -0.2f);
            customPass.SetVector(SpawnPosition, new Vector2(0.5f, 0.5f));
        }

        float EaseOutQuad(float t) => 1 - (1 - t) * (1 - t);
    }
}