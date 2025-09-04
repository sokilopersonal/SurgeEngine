using SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility;
using UGizmo;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SurgeEngine._Source.Code.Infrastructure.Custom.Drawers
{
    public static class TrajectoryDrawer
    {
        public static void DrawTrajectory(Vector3 startPosition, Vector3 direction, Color color, float impulse, float keepVelocityDistance = 0f)
        {
#if UNITY_EDITOR
            int trajectoryPoints = 32;
            float timeStep = 0.1f;
            Vector3 gravity = Physics.gravity.y * Vector3.up;
            int layerMask = 1 << LayerMask.NameToLayer("Default");

            if (keepVelocityDistance > 0f)
            {
                var dir = direction.normalized;
                var newStartPosition = startPosition + dir * keepVelocityDistance;
                UGizmos.DrawLine(startPosition, newStartPosition, Color.red);
                startPosition = newStartPosition;
            }
            
            Trajectory.Calculate(startPosition, direction, impulse, timeStep, trajectoryPoints, gravity, out Vector3[] positions, out Vector3[] velocities);
            for (int i = 0; i < positions.Length - 1; i++)
            {
                Color drawColor = color;

                if (Physics.Linecast(positions[i], positions[i + 1], out RaycastHit hit, layerMask, QueryTriggerInteraction.Ignore))
                {
                    UGizmos.DrawLine(positions[i], hit.point, drawColor);
                    UGizmos.DrawCube(hit.point, Quaternion.identity, Vector3.one * 0.75f, new Color(0.12f, 1f, 0f, 0.59f));
                    
                    break;
                }

                UGizmos.DrawLine(positions[i], positions[i + 1], drawColor);
            }
#endif
        }

        public static void DrawTrickTrajectory(Vector3 startPosition, Vector3 direction, Color color, float impulse)
        {
#if UNITY_EDITOR
            int trajectoryPoints = 120;
            float timeStep = 0.2f;
            int layerMask = 1 << LayerMask.NameToLayer("Default");
            
            Trajectory.CalculateTrick(startPosition, direction, impulse, timeStep, trajectoryPoints, out Vector3[] positions, out Vector3[] velocities);

            for (int i = 0; i < positions.Length - 1; i++)
            {
                if (Physics.Linecast(positions[i], positions[i + 1], out RaycastHit hit, layerMask, QueryTriggerInteraction.Ignore))
                {
                    UGizmos.DrawLine(positions[i], hit.point, color);
                    UGizmos.DrawCube(hit.point, Quaternion.identity, Vector3.one * 0.75f, new Color(0.12f, 1f, 0f, 0.59f));;
                    break;
                }

                UGizmos.DrawLine(positions[i], positions[i + 1], color);
            }
#endif
        }
    }
}