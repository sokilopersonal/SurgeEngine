using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects
{
    public class ModeCollision : ContactBase
    {
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
}