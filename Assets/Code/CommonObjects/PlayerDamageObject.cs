using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class PlayerDamageObject : ContactBase
    {
        public override void Contact(Collider msg)
        {
            base.Contact(msg);

            Actor context = ActorContext.Context;
            context.TakeDamage(this, 1);
        }
    }
}