using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class SpecialJumpData
    {
        public SpecialJumpType type;
        public Vector3 forward;
        public Vector3 up;
        public float dot;

        public SpecialJumpData(SpecialJumpType type, Vector3 forward = default, Vector3 up = default, float dot = 0f)
        {
            this.type = type;
            this.forward = forward;
            this.up = up;
            this.dot = dot;
        }
    }
}