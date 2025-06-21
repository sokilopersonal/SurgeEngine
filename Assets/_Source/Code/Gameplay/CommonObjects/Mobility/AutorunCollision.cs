using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class AutorunCollision : ModeCollision
    {
        private BoxCollider _boxCollider;
        
        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);
            
            if (!CheckFacing(context.transform.forward))
                return;

            var flags = context.Flags;
            if (!flags.HasFlag(FlagType.Autorun))
            {
                flags.AddFlag(new Flag(FlagType.Autorun, null, false));
                flags.AddFlag(new Flag(FlagType.OutOfControl, null, false));
            }
            else
            {
                flags.RemoveFlag(FlagType.Autorun);
                flags.RemoveFlag(FlagType.OutOfControl);
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            if (_boxCollider == null)
                _boxCollider = GetComponent<BoxCollider>();
            
            Gizmos.color = new Color(0.76f, 1f, 0f, 0.39f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(_boxCollider.center, _boxCollider.size);
        }
    }
}