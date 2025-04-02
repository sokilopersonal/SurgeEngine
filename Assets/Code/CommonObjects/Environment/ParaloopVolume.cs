using SurgeEngine.Code.Actor.Sound;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.ActorEffects;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class ParaloopVolume : ContactBase
    {
        private BoxCollider _collider;

        public override void Contact(Collider msg)
        {
            base.Contact(msg);

            ActorBase context = ActorContext.Context;

            if (context.kinematics.Speed >= context.config.minParaloopSpeed)
            {
                context.effects.CreateParaloop();
                context.sounds.GetComponent<ParaloopSound>().Play();
            }
        }

        protected override void OnDrawGizmos()
        {
            if (_collider == null)
                _collider = GetComponent<BoxCollider>();
            
            Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(_collider.center, _collider.size);
        }
    }
}
