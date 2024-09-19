using System;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    [Serializable]
    public class MoveParameters
    {
        [Header("Ground Speed")]
        public float topSpeed = 26;
        public float maxSpeed = 35;
        
        [Header("Acceleration")]
        public float accelRate = 7.85f;
        public AnimationCurve accelCurve;
        
        [Header("Deacceleration")]
        public float minDeacceleration = 9f;
        public float maxDeacceleration = 9f;
        
        [Header("Turning")]
        public float turnSpeed = 12;
        public AnimationCurve turnCurve;
        public float turnSmoothing = 9;
        
        public CastParameters castParameters;
    }
}