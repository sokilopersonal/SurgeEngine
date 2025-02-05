using System;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.EggFighter
{
    public class EGAnimationReference : MonoBehaviour
    {
        public event Action OnStepWalk;
        
        private void WalkStep()
        {
            OnStepWalk?.Invoke();
        }
    }
}