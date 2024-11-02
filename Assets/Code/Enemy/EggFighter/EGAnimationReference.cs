using System;
using UnityEngine;

namespace SurgeEngine.Code.Enemy
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