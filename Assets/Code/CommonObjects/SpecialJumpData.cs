using SurgeEngine.Code.ActorStates;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class SpecialJumpData
    {
        public SpecialJumpType type;
        public Vector3 direction;
        public Vector3 up;
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
        
        public SpecialJumpData(SpecialJumpType type, Transform transform)
        {
            this.type = type;
            this.transform = transform;
        }
    }
}