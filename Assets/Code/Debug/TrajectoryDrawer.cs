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
        public static void DrawTrajectory(Vector3 startPosition, Vector3 direction, Color color, float impulse, float keepVelocity = 0f)
        {
            Vector3 impulseDirection = direction;

            int trajectoryPoints = 240;
            float timeStep = 0.1f;
            int layerMask = 1 << LayerMask.NameToLayer("Default");

            Vector3 position = startPosition;
            Vector3 velocity = impulseDirection.normalized * impulse;
            Vector3 gravity = -35 * Vector3.up;

            float totalTime = 0f;

            for (int j = 0; j < trajectoryPoints; j++)
            {
                totalTime += timeStep;
                
                Vector3 effectiveGravity = totalTime < keepVelocity ? Vector3.zero : gravity;
                Vector3 newPosition = position + velocity * timeStep + 0.5f * effectiveGravity * Mathf.Pow(timeStep, 2);
                
                Color drawColor = totalTime < keepVelocity ? Color.red : color;

                if (Physics.Linecast(position, newPosition, out RaycastHit hit, layerMask))
                {
                    Debug.DrawLine(position, hit.point, drawColor);
                    break;
                }

                Debug.DrawLine(position, newPosition, drawColor);
                position = newPosition;
                
                velocity += effectiveGravity * timeStep;
            }
        }

    }
}