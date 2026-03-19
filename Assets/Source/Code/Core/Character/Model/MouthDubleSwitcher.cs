using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Core.Character.Model
{
    public class MouthDubleSwitcher : MonoBehaviour
    {
        [SerializeField] private Transform mouthReference;

        [Inject] private CharacterBase _character;
        private Transform _cameraTransform;

        private void Awake()
        {
            _cameraTransform = _character.Camera.GetCamera().transform;
        }

        private void LateUpdate()
        {
            float dot = Vector3.Dot(-_cameraTransform.forward, _character.transform.right);

            if (dot < -0.1f)
                mouthReference.localScale = new Vector3(-1, 1, 1);
            else if (dot > 0.1f)
                mouthReference.localScale = new Vector3(1, 1, 1);
        }
    }
}