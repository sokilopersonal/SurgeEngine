using System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data
{
    [Serializable]
    public class PanData
    {
        [HideInInspector] public Vector3 position;
        public float easeTimeEnter = 0.5f;
        public float easeTimeExit = 1f;
        public float fov = 60f;
        public bool allowRotation = true;
    }

    [Serializable]
    public class VerticalPanData : PanData
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
        public Vector3 offset;
        public Vector2 localLookOffset;
        public Vector3 Forward { get; set; }
    }

    public enum RestoreType
    {
        Player,
        Camera
    }
}