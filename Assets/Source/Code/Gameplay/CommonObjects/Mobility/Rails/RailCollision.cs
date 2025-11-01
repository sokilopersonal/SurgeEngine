using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility.Rails
{
    public class RailCollision : StageObject
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