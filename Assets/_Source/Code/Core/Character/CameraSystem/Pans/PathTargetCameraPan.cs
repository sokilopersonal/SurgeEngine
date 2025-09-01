using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.CameraSystem.Pans
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