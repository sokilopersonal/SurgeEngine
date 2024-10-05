using UnityEngine;

namespace SurgeEngine.Code.SurgeDebug
{
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
            Vector3 endPosition = startPosition;

            float totalTime = 0f;

            for (int j = 0; j < trajectoryPoints; j++)
            {
                totalTime += timeStep;
        
                Vector3 effectiveGravity = totalTime < keepVelocity ? Vector3.zero : gravity;
                endPosition = position + velocity * timeStep + 0.5f * effectiveGravity * Mathf.Pow(timeStep, 2);
        
                Color drawColor = totalTime < keepVelocity ? Color.red : color;

                if (Physics.Linecast(position, endPosition, out RaycastHit hit, layerMask, QueryTriggerInteraction.Ignore))
                {
                    Debug.DrawLine(position, hit.point, drawColor);
                    endPosition = hit.point;
                    break;
                }

                Debug.DrawLine(position, endPosition, drawColor);
                position = endPosition;
        
                velocity += effectiveGravity * timeStep;
            }
        }
        
        public static void DrawTrickTrajectory(Vector3 startPosition, Vector3 direction, Color color, float impulse)
        {
            Vector3 impulseDirection = direction;

            int trajectoryPoints = 240;
            float timeStep = 0.1f;
            int layerMask = 1 << LayerMask.NameToLayer("Default");

            Vector3 position = startPosition;
            Vector3 velocity = impulseDirection.normalized * impulse;
            Vector3 gravity = -35f * Vector3.up;

            float totalTime = 0f;
            
            for (int j = 0; j < trajectoryPoints; j++)
            {
                totalTime += timeStep;
                
                Vector3 newPosition = position + velocity * timeStep + 0.5f * gravity * Mathf.Pow(timeStep, 2);
        
                Color drawColor = color;

                if (Physics.Linecast(position, newPosition, out RaycastHit hit, layerMask, QueryTriggerInteraction.Ignore))
                {
                    Debug.DrawLine(position, hit.point, drawColor);
                    break;
                }

                Debug.DrawLine(position, newPosition, drawColor);
                position = newPosition;
        
                velocity += gravity * timeStep;
            }
        }
        
        // public static void DrawTrajectory(Vector3 startPosition, Vector3 direction, float impulse, Color color)
        // {
        //     Vector3 impulseDirection = direction;
        //
        //     int trajectoryPoints = 80;
        //     float timeStep = 0.1f;
        //     int layerMask = 1 << LayerMask.NameToLayer("Default");
        //
        //     Vector3 position = startPosition;
        //     Vector3 velocity = impulseDirection.normalized * impulse;
        //     Vector3 gravity = -27 * Vector3.up;
        //
        //     for (int j = 0; j < trajectoryPoints; j++)
        //     {
        //         Vector3 newPosition = position + velocity * timeStep + 0.5f * gravity * Mathf.Pow(timeStep, 2);
        //     
        //         if (Physics.Linecast(position, newPosition, out RaycastHit hit, layerMask))
        //         {
        //             Debug.DrawLine(position, hit.point, color);
        //             break;
        //         }
        //
        //         Debug.DrawLine(position, newPosition, color);
        //         position = newPosition;
        //         velocity += gravity * timeStep;
        //     }
        // }
    }
}