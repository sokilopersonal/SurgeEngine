using System;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    [Serializable]
    public class HomingParameters
    {
        public float homingSpeed;
        public float homingFindRadius;
        public float homingTime; // when there is no target
        public float homingDistance; //
        public AnimationCurve homingCurve; //
        public LayerMask homingMask;
    }
}