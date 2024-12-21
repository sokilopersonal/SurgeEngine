using SurgeEngine.Code.ActorSystem;
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

            Actor context = ActorContext.Context;

            if (context.kinematics.Speed >= context.config.minParaloopSpeed)
            {
                ParaloopEffect effect = (ParaloopEffect)context.effects.paraloopEffect;
                effect.sonicContext = context;
                effect.startPoint = context.kinematics.Rigidbody.position;
                effect.Toggle(true);
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
