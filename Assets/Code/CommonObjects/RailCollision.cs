using System;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class RailCollision : ContactBase
    {
        private Rail _rail;

        private void Awake()
        {
            _rail = GetComponentInParent<Rail>();
        }

        public override void Contact(Collider msg)
        {
            base.Contact(msg);
            
            _rail.AttachToRail();
        }
    }
}