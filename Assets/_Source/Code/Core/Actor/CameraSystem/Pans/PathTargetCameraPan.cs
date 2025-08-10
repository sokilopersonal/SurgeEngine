using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class PathTargetCameraPan : PathCameraPan
    {
        public PathTargetCameraPan(CharacterBase owner) : base(owner) { }

        protected override Quaternion SetRotation(Quaternion rotation)
        {
            return Quaternion.LookRotation(Character.transform.position - StatePosition);
        }
    }
}