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
        private BoxCollider _boxCollider;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
            
            if (target == null)
            {
                target = GetComponentInChildren<ObjCameraBase>();
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

        protected override void OnDrawGizmos()
        {
            if (target != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, target.transform.position);
            }
            
            if (_boxCollider == null)
                _boxCollider = GetComponent<BoxCollider>();
            
            Gizmos.color = new Color(0.15f, 1f, 0f, 0.1f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(_boxCollider.center, _boxCollider.size);
        }
    }
}