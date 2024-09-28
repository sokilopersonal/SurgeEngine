using System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.CommonObjects
{
    [Serializable]
    public class PathData
    {
        public SplineContainer splineContainer;
        public float outOfControl;
        public float autoRunSpeed;
        public float maxAutoRunSpeed;
    }
}