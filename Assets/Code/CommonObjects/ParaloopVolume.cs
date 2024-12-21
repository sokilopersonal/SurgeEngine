using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorEffects;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class ParaloopVolume : ContactBase
    {
        public override void Contact(Collider msg)
        {
            base.Contact(msg);

            Actor context = ActorContext.Context;

            if (context.kinematics.HorizontalSpeed >= context.config.minParaloopSpeed)
            {
                ParaloopEffect effect = (ParaloopEffect)context.effects.paraloopEffect;
                effect.sonicContext = context;
                effect.startPoint = context.kinematics.Rigidbody.position;
                effect.Toggle(true);
            }
        }
    }
}
