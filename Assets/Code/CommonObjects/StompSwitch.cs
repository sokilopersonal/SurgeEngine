using UnityEngine;
using DG.Tweening;
using FMODUnity;
using UnityEngine.Events;

namespace SurgeEngine
{
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

        private float DOWN_SPEED = 0.5f;
        private Ease DOWN_EASE = Ease.OutBack;
        private Material buttonMaterial;

        int currentState = 0;

        private void Start()
        {
            Material[] mats = meshRenderer.sharedMaterials;
            buttonMaterial = new Material(mats[1]);
            mats[1] = buttonMaterial;
            meshRenderer.sharedMaterials = mats;
        }
        public void Activate(Collider msg)
        {
            if (currentState >= 3)
                return;

            currentState++;

            particle.Play();

            RuntimeManager.PlayOneShot(soundReference, transform.position + Vector3.up);

            float downHeight = 0f;
            
            switch (currentState)
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

                    buttonMaterial.SetFloat("_EmissiveExposureWeight", startEmissive);

                    DOTween.To(() => currentEmissive, x => currentEmissive = x, endEmissive, 0.25f).SetEase(Ease.OutQuad).OnUpdate(() =>
                    {
                        buttonMaterial.SetFloat("_EmissiveExposureWeight", currentEmissive);
                    });

                    RuntimeManager.PlayOneShot(onReference, transform.position + Vector3.up);

                    onActivated.Invoke();
                    break;
            }

            switchTransform.DOKill(true);
            switchTransform.DOLocalMoveY(downHeight, DOWN_SPEED).SetEase(DOWN_EASE);
        }
    }
}
