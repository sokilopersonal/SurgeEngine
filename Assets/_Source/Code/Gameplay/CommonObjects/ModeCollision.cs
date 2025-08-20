using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects
{
    public class ModeCollision : ContactBase
    {
        [SerializeField] protected bool isChangeCamera;
        [SerializeField] private bool isEnabledFromBack = true;
        [SerializeField] private bool isEnabledFromFront = true;

        protected bool CheckFacing(Vector3 dir)
        {
            if (isEnabledFromBack && isEnabledFromFront)
                return true;
            
            float dot = Vector3.Dot(transform.forward, dir);
            
            return isEnabledFromBack && dot > 0 || isEnabledFromFront && dot < 0;
        }
    }

    public class ChangeModeData
    {
        public SplineData Spline { get; private set; }
        public bool IsCameraChange { get; private set; }
        
        public ChangeModeData(SplineData spline, bool isCameraChange)
        {
            Spline = spline;
            IsCameraChange = isCameraChange;
        }
    }
}