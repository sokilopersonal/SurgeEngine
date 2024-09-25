using System;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    [Serializable]
    public class HomingParameters
    {
        public float maxHomingDistance;
        public float homingSpeed;
        public float homingFindRadius;
        public LayerMask homingMask;
    }
}