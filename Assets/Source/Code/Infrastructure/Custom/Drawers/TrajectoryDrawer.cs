using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SurgeEngine.Source.Code.Infrastructure.Custom.Drawers
{
    public static class TrajectoryDrawer
    {
        public static void DrawTrajectory(Vector3 startPosition, Vector3 direction, Color color, float impulse, float keepVelocityDistance = 0f)
        {
#if UNITY_EDITOR
            int trajectoryPoints = 256;
            float timeStep = 0.05f;
            Vector3 gravity = Physics.gravity.y * Vector3.up;
            int layerMask = 1 << LayerMask.NameToLayer("Default");

            if (keepVelocityDistance > 0f)
            {
                var dir = direction.normalized;
                var newStartPosition = startPosition + dir * keepVelocityDistance;
                Handles.color = Color.red;
                Handles.zTest = CompareFunction.LessEqual;
                Handles.DrawLine(startPosition, newStartPosition);
                startPosition = newStartPosition;
            }
            
            Trajectory.Calculate(startPosition, direction, impulse, timeStep, trajectoryPoints, gravity, out Vector3[] positions, out Vector3[] velocities);
            for (int i = 0; i < positions.Length - 1; i++)
            {
                Color drawColor = color;

                if (Physics.Linecast(positions[i], positions[i + 1], out RaycastHit hit, layerMask, QueryTriggerInteraction.Ignore))
                {
                    Handles.color = drawColor;
                    Handles.zTest = CompareFunction.LessEqual;
                    Handles.DrawLine(positions[i], hit.point);

                    Handles.color = new Color(0.12f, 1f, 0f, 0.59f);
                    Handles.DrawWireCube(hit.point, Vector3.one * 0.75f);
                    
                    break;
                }

                Handles.color = drawColor;
                Handles.zTest = CompareFunction.LessEqual;
                Handles.DrawLine(positions[i], positions[i + 1]);
            }
#endif
        }

        public static void DrawTrickTrajectory(Vector3 startPosition, Vector3 direction, Color color, float impulse)
        {
#if UNITY_EDITOR
            int trajectoryPoints = 256;
            float timeStep = 0.05f;
            int layerMask = 1 << LayerMask.NameToLayer("Default");
            
            Trajectory.CalculateTrick(startPosition, direction, impulse, timeStep, trajectoryPoints, out Vector3[] positions, out Vector3[] velocities);

            for (int i = 0; i < positions.Length - 1; i++)
            {
                if (Physics.Linecast(positions[i], positions[i + 1], out RaycastHit hit, layerMask, QueryTriggerInteraction.Ignore))
                {
                    Handles.color = color;
                    Handles.zTest = CompareFunction.LessEqual;
                    Handles.DrawLine(positions[i], hit.point);
                    break;
                }

                Handles.color = color;
                Handles.zTest = CompareFunction.LessEqual;
                Handles.DrawLine(positions[i], positions[i + 1]);
            }
#endif
        }
    }
}