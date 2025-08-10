using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Code.Gameplay.CommonObjects
{
    public class HomingTarget : MonoBehaviour
    {
        [SerializeField] private float distanceThreshold = 0.7f;
        public float DistanceThreshold => distanceThreshold;
        
        public UnityEvent<CharacterBase> OnTargetReached;

        public void SetDistanceThreshold(float threshold)
        {
            distanceThreshold = Mathf.Abs(threshold);
            if (distanceThreshold < 0.1f) 
                distanceThreshold = 0.1f;
        }
    }
}