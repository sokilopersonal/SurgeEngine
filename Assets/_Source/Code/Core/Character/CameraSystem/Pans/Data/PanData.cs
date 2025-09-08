using System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine._Source.Code.Core.Character.CameraSystem.Pans.Data
{
    [Serializable]
    public class PanData
    {
        [HideInInspector] public Vector3 position;
        public float easeTimeEnter = 0.5f;
        public float easeTimeExit = 1f;
        public float fov = 60f;
        public bool allowRotation = true;
        [Tooltip("Only works on cameras that are just modifications of a normal camera")] public bool isCollision = true;
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
        public Vector3 Forward { get; set; }
    }

    [Serializable]
    public class PathPanData : PanData
    {
        public SplineContainer container;
        public float offsetOnEye = -10f;
    }
}