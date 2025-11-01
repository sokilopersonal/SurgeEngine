using System;
using UnityEngine.Splines;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes
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