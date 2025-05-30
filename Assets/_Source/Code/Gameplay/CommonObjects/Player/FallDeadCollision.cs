using System;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Player
{
    [RequireComponent(typeof(BoxCollider))]
    public class FallDeadCollision : ContactBase
    {
        private BoxCollider _collider;

        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);

            ActorInput input = context.Input;
            input.playerInput.enabled = false;
            
            context.Camera.stateMachine.SetState<FallCameraState>();
            context.OnDiedInvoke(context, true);
        }

        private void OnDrawGizmosSelected()
        {
            if (_collider == null)
                _collider = GetComponent<BoxCollider>();
            
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawCube(_collider.center, _collider.size);
        }
    }
}