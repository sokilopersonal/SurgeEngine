using SurgeEngine.Code.Core.Actor.States;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class SpecialJumpData
    {
        public SpecialJumpType type;
        public Vector3 direction;
        public Vector3 up;
        public float outOfControl;
        public Transform transform;

        public SpecialJumpData(SpecialJumpType type)
        {
            this.type = type;
        }
        
        public SpecialJumpData(SpecialJumpType type, Vector3 direction, Vector3 up)
        {
            this.type = type;
            this.direction = direction;
            this.up = up;
        }
        
        public SpecialJumpData(SpecialJumpType type, Transform transform, float outOfControl)
        {
            this.type = type;
            this.transform = transform;
            this.outOfControl = outOfControl;
        }
    }
}