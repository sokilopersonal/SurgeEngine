using SurgeEngine.Code.Gameplay.CommonObjects.Mobility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SurgeEngine.Code.Infrastructure.Custom.Drawers
{
    public static class TrajectoryDrawer
    {
        public static void DrawTrajectory(Vector3 startPosition, Vector3 direction, Color color, float impulse, float keepVelocity = 0f)
        {
            int trajectoryPoints = 32;
            float timeStep = 0.1f;
            Vector3 gravity = Physics.gravity.y * Vector3.up;
            int layerMask = 1 << LayerMask.NameToLayer("Default");

            if (keepVelocity > 0f)
            {
                Vector3 newStartPosition = startPosition + direction * keepVelocity * impulse;
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
        }

        public static void DrawTrickTrajectory(Vector3 startPosition, Vector3 direction, Color color, float impulse)
        {
            int trajectoryPoints = 240;
            float timeStep = 0.1f;
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
        }
    }
}