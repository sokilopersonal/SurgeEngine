using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans.Data;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.CameraSystem
{
    public class CameraBlending
    {
        public float BlendFactor { get; private set; }

        private CameraData _from;

        public void Complete() => BlendFactor = 1f;
        public void Reset() => BlendFactor = 0f;

        public void RememberFrom(Vector3 relativePosition, Quaternion rotation, float fov)
        {
            _from = new CameraData
            {
                Position = relativePosition,
                Rotation = rotation,
                FOV = fov,
            };
        }

        public void Tick(float dt, CameraEaseData easeData, bool isExit)
        {
            float easeTime = isExit ? easeData.LeaveTime : easeData.EnterTime;

            if (easeTime > 0)
                BlendFactor += dt / easeTime;
            else
                BlendFactor = 1f;

            BlendFactor = Mathf.Clamp01(BlendFactor);
        }

        public (Vector3 position, Quaternion rotation, float fov) Evaluate(
            Vector3 targetPos, Quaternion targetRot, float targetFov, Vector3 characterPos)
        {
            float t = Easings.Get(Easing.Gens, BlendFactor);

            if (BlendFactor >= 1f)
                return (targetPos, targetRot, targetFov);

            Vector3 diff = targetPos - characterPos;
            Vector3 pos = Vector3.Slerp(_from.Position, diff, t) + characterPos;
            Quaternion rot = Quaternion.Slerp(_from.Rotation, targetRot, t);
            float fov = Mathf.Lerp(_from.FOV, targetFov, t);

            return (pos, rot, fov);
        }
    }
}