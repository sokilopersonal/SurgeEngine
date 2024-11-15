using System;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    [Serializable]
    public class PanData
    {
        public Vector3 position;
        public float easeTimeEnter = 0.5f;
        public float easeTimeExit = 0.5f;
        public float fov = 60f;
        public bool allowRotation = true;
    }

    [Serializable]
    public class VerticalPanData : PanData
    {
        public float groundOffset = 4f;
        public float yOffset = 1f;
        public Vector3 forward;
    }

    [Serializable]
    public class FixPanData : PanData
    {
        public Quaternion target;
    }
}