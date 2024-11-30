using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class SpecialJumpData
    {
        public SpecialJumpType type;

        public SpecialJumpData(SpecialJumpType type)
        {
            this.type = type;
        }
    }
}