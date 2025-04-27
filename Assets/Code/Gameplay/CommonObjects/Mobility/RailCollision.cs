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

        public override void Contact(Collider msg)
        {
            base.Contact(msg);
            
            _rail.AttachToRail();
        }
    }
}