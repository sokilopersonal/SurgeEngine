using System;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    [Serializable]
    public class CameraParameters
    {
        public string name;
        
        [Header("Follow")]
        public float distance = 2.4f;
        [Min(0)] public float distanceDuration = 1f;
        public Easing distanceEasing = Easing.InOutSine;
        [Range(1, 3f)] public float followPower = 0.125f;
        public float timeToStartFollow = 2f;
        
        [Header("FOV")]
        public float fov = 60f;
        [Min(0)] public float fovDuration = 1f;
        public Easing fovEasing = Easing.InOutSine;
    }
}