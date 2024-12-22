using System;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
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
        public Quaternion target;
    }
}