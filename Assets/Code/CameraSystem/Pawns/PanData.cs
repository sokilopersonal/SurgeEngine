using System;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem.Pawns
{
    [Serializable]
    public class PanData
    {
        public Vector3 position;
        public float easeInTime = 0.5f;
        public float easeOutTime = 0.5f;
        public float fov = 60f;
    }
}