using System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans.Data
{
    [Serializable]
    public class PanData
    {
        [HideInInspector] public Vector3 position;
        public float easeTimeEnter = 1;
        public float easeTimeLeave = 1;
        public float fov = 60f;

        [Tooltip("Only works on cameras that are just modifications of a normal camera")] 
        public bool isCollision = true;
    }

    [Serializable]
    public class ParallelPanData : PanData
    {
        public float distance = 4f;
        public float yOffset = 0.25f;
        [HideInInspector] public Vector3 forward;
    }

    [Serializable]
    public class FixPanData : PanData
    {
        [HideInInspector] public Quaternion target;
    }

    [Serializable]
    public class NormalPanData : PanData
    {
        public float distance = 4f;
        public float yOffset = 0.25f;
    }

    [Serializable]
    public class PointPanData : PanData
    {
        public float distance = 4f;
        public float yOffset = 0.25f;
        public Transform target;
    }

    [Serializable]
    public class PathPanData : PanData
    {
        public SplineContainer container;
        public float offsetOnEye = -10f;
    }
}