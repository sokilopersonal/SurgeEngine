using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    [SelectionBase]
    public class StompSwitch : MonoBehaviour
    {
        private static readonly int EmissiveExposureWeight = Shader.PropertyToID("_EmissiveExposureWeight");

        [Header("Main")]
        [SerializeField] private Transform switchTransform;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private ParticleSystem particle;
        
        [Header("Switch Events")]
        public UnityEvent onActivated;

        [Header("Sounds")]
        [SerializeField] private EventReference soundReference;
        [SerializeField] private EventReference onReference;

        private const float DownSpeed = 0.5f;
        private const Ease DownEase = Ease.OutBack;
        private Material _buttonMaterial;

        private int _currentState;

        private void Awake()
        {
            Material[] materials = meshRenderer.materials;
            _buttonMaterial = materials[1];
            meshRenderer.materials = materials;
            _buttonMaterial.SetFloat(EmissiveExposureWeight, 1.0f);
        }
        
        public void Activate()
        {
            if (_currentState >= 3)
                return;

            _currentState++;

            particle.Play();

            RuntimeManager.PlayOneShot(soundReference, transform.position + Vector3.up);

            float downHeight = 0f;
            
            switch (_currentState)
            {
                case 1:
                    downHeight = -0.75f;
                    break;
                case 2:
                    downHeight = -1.5f;
                    break;
                case 3:
                    downHeight = -2.25f;

                    float startEmissive = 1f;
                    float endEmissive = 0f;

                    float currentEmissive = startEmissive;

                    _buttonMaterial.SetFloat(EmissiveExposureWeight, startEmissive);

                    DOTween.To(() => currentEmissive, x => currentEmissive = x, endEmissive, 0.25f).SetEase(Ease.OutQuad).OnUpdate(() =>
                    {
                        _buttonMaterial.SetFloat(EmissiveExposureWeight, currentEmissive);
                    });

                    RuntimeManager.PlayOneShot(onReference, transform.position + Vector3.up);

                    onActivated.Invoke();
                    break;
            }

            switchTransform.DOKill(true);
            switchTransform.DOLocalMoveY(downHeight, DownSpeed).SetEase(DownEase);
        }
    }
}
