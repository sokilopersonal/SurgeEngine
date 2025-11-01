using System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter
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