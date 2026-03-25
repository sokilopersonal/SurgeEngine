using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ChangeCameraVolume : StageObject
    {
        [SerializeField] private ObjCameraBase target;
        [SerializeField] private float easeTimeEnter = 1f;
        [SerializeField] private float easeTimeLeave = 1f;
        [SerializeField] private int priority;
        public ObjCameraBase Target => target;
        public float EaseTimeEnter => easeTimeEnter;
        public float EaseTimeLeave => easeTimeLeave;
        public int Priority => priority;

        private CharacterBase _character;

        private void OnDisable()
        {
            if (_character != null)
            {
                _character.Camera.StateMachine.UnregisterVolume(this);
                _character = null;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (target && other.transform.TryGetComponent(out CharacterBase character))
            {
                _character = character;
                _character.Camera.StateMachine.RegisterVolume(this);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (target && other.transform.TryGetComponent(out CharacterBase character))
            {
                character.Camera.StateMachine.UnregisterVolume(this);
                _character = null;
            }
        }

        private void OnDrawGizmos()
        {
            if (target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, target.transform.position);
            }
        }
    }
}