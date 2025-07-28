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
            if (_actor != null)
            {
                _actor.Camera.StateMachine.UnregisterVolume(this);
                _actor = null;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (target && other.transform.TryGetComponent(out ActorBase actor))
            {
                _actor = actor;
                _actor.Camera.StateMachine.RegisterVolume(this);
            }
        }

        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);

            if (target)
            {
                //context.Camera.StateMachine.RegisterVolume(this);
            }
        }
        
        private void OnTriggerExit(Collider other)
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