using DG.Tweening;
using FMODUnity;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    [SelectionBase]
    public class StompSwitch : MonoBehaviour, IPointMarkerLoader
    {
        private static readonly int EmissiveColor = Shader.PropertyToID("_EmissiveColor");

        [Header("Main")]
        [SerializeField] private Transform switchTransform;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private new Light light;
        
        [Header("Switch Events")]
        public UnityEvent onActivated;

        [Header("Sounds")]
        [SerializeField] private EventReference soundReference;
        [SerializeField] private EventReference onReference;

        private const float DownSpeed = 0.5f;
        private const Ease DownEase = Ease.OutBack;
        private Material _buttonMaterial;
        private float _startLightIntensity;

        private int _currentState;

        private void Awake()
        {
            Material[] materials = meshRenderer.materials;
            _buttonMaterial = materials[1];
            meshRenderer.materials = materials;
            _buttonMaterial.SetColor(EmissiveColor, Color.cyan);
            _startLightIntensity = light.intensity;
            light.intensity = 0;
        }
        
        public void Activate()
        {
            if (_currentState >= 3)
                return;

            _currentState++;

            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
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

                    float startIntensity = 0f;
                    float endIntensity = 20000f;

                    float currentEmissive = startIntensity;

                    _buttonMaterial.SetColor(EmissiveColor, Color.cyan * startIntensity);
                    DOTween.To(() => currentEmissive, x => currentEmissive = x, endIntensity, 0.25f).SetEase(Ease.OutQuad).OnUpdate(() =>
                    {
                        _buttonMaterial.SetColor(EmissiveColor, Color.cyan * currentEmissive);
                    });
                    
                    light.DOIntensity(_startLightIntensity, 0.25f).SetEase(Ease.OutQuad);

                    RuntimeManager.PlayOneShot(onReference, transform.position + Vector3.up);

                    onActivated.Invoke();
                    break;
            }

            switchTransform.DOKill(true);
            switchTransform.DOLocalMoveY(downHeight, DownSpeed).SetEase(DownEase);
        }

        public void Load()
        {
            _currentState = 0;
            switchTransform.localPosition = Vector3.zero;
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _buttonMaterial.SetFloat(EmissiveColor, 1.0f);
        }
    }
}
