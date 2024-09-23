using UnityEngine;

namespace SurgeEngine.Code.SurgeDebug
{
    public enum TrajectoryObject
    {
        JumpCollision,
        JumpPanel,
        Spring,
        TrickJumper, // for the future QTE ramp
    }
    
    public static class TrajectoryDrawer
    {
        public static void DrawTrajectory(Vector3 startPosition, Vector3 direction, float impulse, Color color)
        {
            Vector3 impulseDirection = direction;

            int trajectoryPoints = 80;
            float timeStep = 0.1f;
            int layerMask = 1 << LayerMask.NameToLayer("Default");

            Vector3 position = startPosition;
            Vector3 velocity = impulseDirection.normalized * impulse;
            Vector3 gravity = -27 * Vector3.up;

            for (int j = 0; j < trajectoryPoints; j++)
            {
                Vector3 newPosition = position + velocity * timeStep + 0.5f * gravity * Mathf.Pow(timeStep, 2);
            
                if (Physics.Linecast(position, newPosition, out RaycastHit hit, layerMask))
                {
                    Debug.DrawLine(position, hit.point, color);
                    break;
                }

                Debug.DrawLine(position, newPosition, color);
                position = newPosition;
                velocity += gravity * timeStep;
            }
        }
    }
}