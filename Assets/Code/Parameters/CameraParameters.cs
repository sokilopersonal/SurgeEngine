using System;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    [Serializable]
    public class CameraParameters
    {
        [Header("Target")]
        public Transform target;
        
        [Header("Collision")]
        public LayerMask collisionMask;
        public float collisionRadius = 0.2f;
        
        [Header("Follow")]
        public float distance = 2.4f;
        public float distanceChangeSpeed = 2f;
        [Range(0, 0.05f)] public float followPower = 0.0225f;
        public float timeToStartFollow = 2f;
        
        [Header("FOV")]
        public float fov = 60f;
        public float fovChangeSpeed = 3f;
    }
}