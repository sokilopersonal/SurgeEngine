using SurgeEngine._Source.Code.Core.Character.CameraSystem.Pans;
using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Player
{
    [RequireComponent(typeof(BoxCollider))]
    public class FallDeadCollision : StageObject
    {
        private BoxCollider _collider;

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            CharacterInput input = context.Input;
            input.playerInput.enabled = false;
            
            context.Camera.StateMachine.SetState<FallCameraState>();
            context.Life.OnDied?.Invoke(context);
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