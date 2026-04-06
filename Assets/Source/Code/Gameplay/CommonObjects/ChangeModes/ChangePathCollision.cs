using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangePathCollision : ModeCollision
    {
        [SerializeField] private DominantSide dominantSide = DominantSide.Left;
        
        // TODO: Implement changing path on other modes?
        protected override SplineTag SplineTagFilter => SplineTag.SideView;

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);
            
            context.Kinematics.Mode.Path2D?.SetSpline(new SplineData(Container, context.Rigidbody.position, dominantSide));
        }

        protected override void SetMode(CharacterBase ctx)
        {
            
        }

        protected override void RemoveMode(CharacterBase ctx)
        {
            
        }
    }
}