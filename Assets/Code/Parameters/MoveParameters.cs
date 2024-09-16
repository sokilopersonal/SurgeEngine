using System;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    [Serializable]
    public class MoveParameters
    {
        public float topSpeed = 26;
        public float turnSpeed = 12;
        public AnimationCurve turnCurve;
        public float turnSmoothing = 9;
        public float accelRate = 7.85f;
        public AnimationCurve accelCurve;
    }
}