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
        public Material active;

        [Space(25)]

        [Header("Switch Events")]
        [Space(10)]
        public UnityEvent onActivated;

        [Space(25)]

        [Header("Sounds")]
        [Space(10)]
        public EventReference soundReference;
        public EventReference onReference;

        [Space(25)]

        [Header("Tweening")]
        [Space(10)]
        public float DOWN_SPEED = 0.5f;
        public Ease DOWN_EASE = Ease.OutBack;

        int currentState = 0;
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
                    downHeight = -0.8f;
                    break;
                case 2:
                    downHeight = -1.6f;
                    break;
                case 3:
                    downHeight = -2.5f;

                    Material[] mats = meshRenderer.sharedMaterials;
                    mats[1] = active;
                    meshRenderer.sharedMaterials = mats;

                    RuntimeManager.PlayOneShot(onReference, transform.position + Vector3.up);

                    onActivated.Invoke();
                    break;
            }

            switchTransform.DOKill(true);
            switchTransform.DOLocalMoveY(downHeight, DOWN_SPEED).SetEase(DOWN_EASE);
        }
    }
}
