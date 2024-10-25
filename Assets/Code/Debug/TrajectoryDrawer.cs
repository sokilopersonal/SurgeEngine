using SurgeEngine.Code.CommonObjects;
using UnityEngine;

namespace SurgeEngine.Code.SurgeDebug
{
    public static class TrajectoryDrawer
    {
        public static void DrawTrajectory(Vector3 startPosition, Vector3 direction, Color color, float impulse, float keepVelocity = 0f)
        {
            int trajectoryPoints = 240;
            float timeStep = 0.1f;
            Vector3 gravity = Physics.gravity.y * Vector3.up;
            int layerMask = 1 << LayerMask.NameToLayer("Default");
            
            Trajectory.Calculate(startPosition, direction, impulse, timeStep, trajectoryPoints, gravity, out Vector3[] positions, out Vector3[] velocities);

            for (int i = 0; i < positions.Length - 1; i++)
            {
                Color drawColor = color;

                if (Physics.Linecast(positions[i], positions[i + 1], out RaycastHit hit, layerMask, QueryTriggerInteraction.Ignore))
                {
                    Debug.DrawLine(positions[i], hit.point, drawColor);
                    break;
                }

                Debug.DrawLine(positions[i], positions[i + 1], drawColor);
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
                    Debug.DrawLine(positions[i], hit.point, color);
                    break;
                }

                Debug.DrawLine(positions[i], positions[i + 1], color);
            }
        }
    }
}