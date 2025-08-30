using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Model
{
    public class MouthDubleSwitcher : MonoBehaviour
    {
        [SerializeField] Transform mouthReference;

        private Transform _cameraTransform;
        private CharacterBase _character;

        private void Start()
        {
            _character = CharacterContext.Context;
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