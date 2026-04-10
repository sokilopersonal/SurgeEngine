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

            Vector3 fromDiff = _from.Position;
            Vector3 targetDiff = targetPos - characterPos;

            Vector3 pos = DirectionalSlerp(fromDiff, targetDiff, t) + characterPos;
            Quaternion rot = SmoothedSlerp(_from.Rotation, targetRot, t);
            float fov = Mathf.Lerp(_from.FOV, targetFov, t);

            return (pos, rot, fov);
        }

        private static Vector3 DirectionalSlerp(Vector3 from, Vector3 to, float t)
        {
            float fromLen = from.magnitude;
            float toLen = to.magnitude;

            if (fromLen < 1e-5f && toLen < 1e-5f)
                return Vector3.zero;

            Vector3 fromDir = fromLen > 1e-5f ? from / fromLen : to / toLen;
            Vector3 toDir = toLen > 1e-5f ? to / toLen : from / fromLen;

            Vector3 dir = Vector3.Slerp(fromDir, toDir, t);
            float len = Mathf.Lerp(fromLen, toLen, t);

            return dir * len;
        }

        private static Quaternion SmoothedSlerp(Quaternion q1, Quaternion q2, float t)
        {
            float dot = q1.x * q2.x + q1.y * q2.y + q1.z * q2.z + q1.w * q2.w;

            if (dot < 0f)
            {
                q2 = new Quaternion(-q2.x, -q2.y, -q2.z, -q2.w);
                dot = -dot;
            }

            dot = Mathf.Clamp(dot, -1f, 1f);

            const float threshold = 0.9998f;
            if (dot > threshold)
            {
                Quaternion result = new Quaternion(
                    q1.x + t * (q2.x - q1.x),
                    q1.y + t * (q2.y - q1.y),
                    q1.z + t * (q2.z - q1.z),
                    q1.w + t * (q2.w - q1.w)
                );
                return Normalize(result);
            }

            float omega = Mathf.Acos(dot);
            float sinOmega = Mathf.Sin(omega);
            float scale0 = Mathf.Sin((1f - t) * omega) / sinOmega;
            float scale1 = Mathf.Sin(t * omega) / sinOmega;

            return new Quaternion(
                scale0 * q1.x + scale1 * q2.x,
                scale0 * q1.y + scale1 * q2.y,
                scale0 * q1.z + scale1 * q2.z,
                scale0 * q1.w + scale1 * q2.w
            );
        }

        private static Quaternion Normalize(Quaternion q)
        {
            float mag = Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
            if (mag < 1e-8f) return Quaternion.identity;
            return new Quaternion(q.x / mag, q.y / mag, q.z / mag, q.w / mag);
        }
    }
}