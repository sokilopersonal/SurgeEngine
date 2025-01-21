using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Code.CommonObjects
{
    [SelectionBase]
    public class StompSwitch : MonoBehaviour
    {
        [Header("Main")]
        [Space(10)]
        [SerializeField] Transform switchTransform;
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] ParticleSystem particle;

        [Space(25)]

        [Header("Switch Events")]
        [Space(10)]
        public UnityEvent onActivated;

        [Space(25)]

        [Header("Sounds")]
        [Space(10)]
        public EventReference soundReference;
        public EventReference onReference;

        private readonly float _downSpeed = 0.5f;
        private readonly Ease _downEase = Ease.OutBack;
        private Material _buttonMaterial;

        int _currentState;

        private void Start()
        {
            Material[] mats = meshRenderer.sharedMaterials;
            _buttonMaterial = new Material(mats[1]);
            mats[1] = _buttonMaterial;
            meshRenderer.sharedMaterials = mats;
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

                    _buttonMaterial.SetFloat("_EmissiveExposureWeight", startEmissive);

                    DOTween.To(() => currentEmissive, x => currentEmissive = x, endEmissive, 0.25f).SetEase(Ease.OutQuad).OnUpdate(() =>
                    {
                        _buttonMaterial.SetFloat("_EmissiveExposureWeight", currentEmissive);
                    });

                    RuntimeManager.PlayOneShot(onReference, transform.position + Vector3.up);

                    onActivated.Invoke();
                    break;
            }

            switchTransform.DOKill(true);
            switchTransform.DOLocalMoveY(downHeight, _downSpeed).SetEase(_downEase);
        }
    }
}
