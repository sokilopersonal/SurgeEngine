using System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.Enemy.EggFighter
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