using System;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pawns.Data
{
    [Serializable]
    public class PanData
    {
        [HideInInspector] public Vector3 position;
        public float easeTimeEnter = 0.5f;
        public float easeTimeExit = 1f;
        public float fov = 60f;
        public bool allowRotation = true;
        public RestoreType restoreType = RestoreType.Player;

        public Vector3 RestoreDirection()
        {
            var context = ActorContext.Context;
            return restoreType == RestoreType.Player ? context.transform.forward : context.camera.GetCameraTransform().transform.forward;
        }
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

    public enum RestoreType
    {
        Player,
        Camera
    }
}