using UnityEngine;

namespace SurgeEngine.Code.SurgeDebug
{
    public static class TrajectoryDrawer
    {
        public static void DrawTrajectoryWithOneImpulse(Vector3 startPosition, Vector3 direction, float impulse)
        {
            Vector3 impulseDirection = direction;
            float[] impulses = { impulse };
            Color[] colors = { Color.red };
    
            int trajectoryPoints = 80;
            float timeStep = 0.1f;
            int layerMask = 1 << LayerMask.NameToLayer("Default");
    
            for (int i = 0; i < impulses.Length; i++)
            {
                Vector3 position = startPosition;
                Vector3 velocity = impulseDirection.normalized * impulses[i];
                Vector3 gravity = -27 * Vector3.up;
        
                for (int j = 0; j < trajectoryPoints; j++)
                {
                    Vector3 newPosition = position + velocity * timeStep + 0.5f * gravity * Mathf.Pow(timeStep, 2);
            
                    if (Physics.Linecast(position, newPosition, out RaycastHit hit, layerMask))
                    {
                        Debug.DrawLine(position, hit.point, colors[i]);
                        break;
                    }
                    
                    Debug.DrawLine(position, newPosition, colors[i]);
                    position = newPosition;
                    velocity += gravity * timeStep;
                }
            }
        }
        
        public static void DrawTrajectoryWithTwoImpulses(Vector3 startPosition, Vector3 direction, float impulseOnNormal, float impulseOnBoost)
        {
            Vector3 impulseDirection = direction;
            float[] impulses = { impulseOnNormal, impulseOnBoost };
            Color[] colors = { Color.white, Color.blue };
    
            int trajectoryPoints = 80;
            float timeStep = 0.1f;
            int layerMask = 1 << LayerMask.NameToLayer("Default");
    
            for (int i = 0; i < impulses.Length; i++)
            {
                Vector3 position = startPosition;
                Vector3 velocity = impulseDirection.normalized * impulses[i];
                Vector3 gravity = -27 * Vector3.up;
        
                for (int j = 0; j < trajectoryPoints; j++)
                {
                    Vector3 newPosition = position + velocity * timeStep + 0.5f * gravity * Mathf.Pow(timeStep, 2);
            
                    if (Physics.Linecast(position, newPosition, out RaycastHit hit, layerMask))
                    {
                        Debug.DrawLine(position, hit.point, colors[i]);
                        break;
                    }
                    
                    Debug.DrawLine(position, newPosition, colors[i]);
                    position = newPosition;
                    velocity += gravity * timeStep;
                }
            }
        }
    }
}