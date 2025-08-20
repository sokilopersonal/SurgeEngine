using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.ChangeModes
{
    public abstract class ChangeMode3D : ModeCollision
    {
        [Tooltip("Prevents the player from running past the end of the path")]
        [SerializeField] protected bool isLimitEdge = true;
        [SerializeField] protected float pathCorrectionForce = 1f;
    }
    
    public class ChangeMode3DData : ChangeModeData
    {
        public bool IsLimitEdge { get; private set; }
        public float PathCorrectionForce { get; private set; }
        
        public ChangeMode3DData(SplineData spline, bool isCameraChange, bool isLimitEdge, float pathCorrectionForce) : base(spline, isCameraChange)
        {
            IsLimitEdge = isLimitEdge;
            PathCorrectionForce = pathCorrectionForce;
        }
    }
}