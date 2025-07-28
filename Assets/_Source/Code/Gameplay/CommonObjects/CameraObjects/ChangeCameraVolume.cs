using System;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ChangeCameraVolume : ContactBase
    {
        [SerializeField] private ObjCameraBase target;
        public ObjCameraBase Target => target;
        [SerializeField] private int priority;
        public int Priority => priority;

        private ActorBase _actor;

        private void Awake()
        {
            if (target == null)
            {
                target = GetComponentInChildren<ObjCameraBase>();
            }
        }

        private void OnDisable()
        {
            Unregister();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.transform.TryGetComponent(out _actor))
            {
                Register();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Unregister();
        }

        private void Register()
        {
            if (target)
            {
                _actor.Camera.StateMachine.RegisterVolume(this);
            }
        }

        private void Unregister()
        {
            if (target && _actor)
            {
                _actor.Camera.StateMachine.UnregisterVolume(this);
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