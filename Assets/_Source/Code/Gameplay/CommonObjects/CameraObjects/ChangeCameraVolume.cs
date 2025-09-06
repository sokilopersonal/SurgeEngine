using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ChangeCameraVolume : StageObject
    {
        [SerializeField] private ObjCameraBase target;
        public ObjCameraBase Target => target;
        [SerializeField] private int priority;
        public int Priority => priority;

        private CharacterBase _character;

        private void Awake()
        {
            if (target == null)
            {
                target = GetComponentInChildren<ObjCameraBase>();
            }
        }

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
            if (target && other.transform.TryGetComponent(out CharacterBase actor))
            {
                _character = actor;
                _character.Camera.StateMachine.RegisterVolume(this);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (target && other.transform.TryGetComponent(out CharacterBase actor))
            {
                actor.Camera.StateMachine.UnregisterVolume(this);
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