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
    }

    [Serializable]
    public class VerticalPanData : PanData
    {
        public float groundOffset;
    }
}