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

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);
            
            //_rail.AttachToRail();
        }
    }
}