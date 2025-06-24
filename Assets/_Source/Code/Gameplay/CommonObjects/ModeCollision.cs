using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects
{
    public class ModeCollision : ContactBase
    {
        [SerializeField] private bool haveToFaceTheDirection = true;
        
        protected bool CheckFacing(Vector3 dir)
        {
            if (!haveToFaceTheDirection)
                return true;
            
            float dot = Vector3.Dot(transform.forward, dir);
            return dot > 0.02f;
        }
    }
}