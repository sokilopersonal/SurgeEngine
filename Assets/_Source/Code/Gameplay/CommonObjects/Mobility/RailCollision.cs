using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class RailCollision : ContactBase
    {
        private Rail _rail;

        protected override void Awake()
        {
            _rail = GetComponentInParent<Rail>();
        }

        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);
            
            //_rail.AttachToRail();
        }
    }
}