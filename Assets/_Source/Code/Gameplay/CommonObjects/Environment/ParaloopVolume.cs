using SurgeEngine.Code.Core.Actor.Sound;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Environment
{
    public class ParaloopVolume : ContactBase
    {
        private BoxCollider _collider;

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);
            
            if (context.Kinematics.Speed >= context.Config.minParaloopSpeed)
            {
                context.Effects.CreateParaloop();
                context.Sounds.GetComponent<ParaloopSound>().Play();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_collider == null)
                _collider = GetComponent<BoxCollider>();
            
            Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(_collider.center, _collider.size);
        }
    }
}
