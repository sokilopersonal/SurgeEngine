using System;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    [Serializable]
    public class MoveParameters
    {
        public float topSpeed = 26;
        public float maxSpeed = 35;
        public float turnSpeed = 12;
        public AnimationCurve turnCurve;
        public float turnSmoothing = 9;
        public float accelRate = 7.85f;
        public AnimationCurve accelCurve;
        public CastParameters castParameters;
    }
}