using SurgeEngine.Code.Custom;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace SurgeEngine.Code.ActorEffects
{
    public class BoostDistortion : MonoBehaviour
    {
        [SerializeField] private Material distortionMaterial;
        [SerializeField] private CustomPassVolume volume;
        
        [SerializeField] private Easing easing;
        [SerializeField] private float duration;
        
        private static readonly int WaveTime = Shader.PropertyToID("_WaveTime");
        private static readonly int SpawnPosition = Shader.PropertyToID("_SpawnPosition");
        
        private float _timer;
        private bool _isPlaying;

        private void Awake()
        {
            distortionMaterial = Instantiate(distortionMaterial);

            CustomPass pass = volume.customPasses[0];
            FullScreenCustomPass fPass = pass as FullScreenCustomPass;
            fPass.fullscreenPassMaterial = distortionMaterial;
        }

        private void Update()
        {
            if (_isPlaying)
            {
                distortionMaterial.SetFloat(WaveTime, Mathf.Lerp(-0.4f, 1 * GetAspect(), Easings.Get(easing, _timer)));
            
                if (_timer < duration)
                {
                    _timer += Time.deltaTime;
                }
                else
                {
                    _timer = 0;
                    _isPlaying = false;
                }
            }
            else
            {
                distortionMaterial.SetFloat(WaveTime, -0.4f);
            }
        }

        public void Play(Vector3 viewportPosition)
        {
            distortionMaterial.SetVector(SpawnPosition, viewportPosition);

            _timer = 0;
            _isPlaying = true;
        }

        private float GetAspect()
        {
            float w = Screen.width;
            float h = Screen.height;
            return w / h;
        }
    }
}