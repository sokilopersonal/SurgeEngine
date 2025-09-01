using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility.Rails
{
    public class RailCollision : ContactBase
    {
        private Rail _rail;

        private void Awake()
        {
            _rail = GetComponentInParent<Rail>();
        }

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);
            
            //_rail.AttachToRail();
        }
    }
}