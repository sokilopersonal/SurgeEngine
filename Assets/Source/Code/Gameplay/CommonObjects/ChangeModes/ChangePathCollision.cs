using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangePathCollision : ModeCollision
    {
        [SerializeField] private DominantSide dominantSide = DominantSide.Left;

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);
            
            //context.Kinematics.Mode.Mode2D?.SetSpline(new SplineData(container, context.Rigidbody.position));
        }

        protected override void SetMode(CharacterBase ctx)
        {
            
        }

        protected override void RemoveMode(CharacterBase ctx)
        {
            
        }
    }
}