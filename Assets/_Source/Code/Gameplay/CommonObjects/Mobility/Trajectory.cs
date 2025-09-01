using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public static class Trajectory
    {
        public static void Calculate(Vector3 startPosition, Vector3 direction, float impulse, float timeStep, int trajectoryPoints, Vector3 gravity, out Vector3[] positions, out Vector3[] velocities)
        {
            positions = new Vector3[trajectoryPoints];
            velocities = new Vector3[trajectoryPoints];

            Vector3 position = startPosition;
            Vector3 velocity = direction.normalized * impulse;

            for (int i = 0; i < trajectoryPoints; i++)
            {
                positions[i] = position;
                velocities[i] = velocity;

                Vector3 newPosition = position + velocity * timeStep + gravity * (0.5f * Mathf.Pow(timeStep, 2));

                position = newPosition;
                velocity += gravity * timeStep;
            }
        }

        public static void CalculateTrick(Vector3 startPosition, Vector3 direction, float impulse, float timeStep, int trajectoryPoints, out Vector3[] positions, out Vector3[] velocities)
        {
            Vector3 gravity = UnityEngine.Physics.gravity;

            Calculate(startPosition, direction, impulse, timeStep, trajectoryPoints, gravity, out positions, out velocities);
        }
        
        public static Vector3 GetArcPosition(Vector3 startPosition, Vector3 direction, float impulse)
        {
            int trajectoryPoints = 240;
            float timeStep = 0.1f;
            int layerMask = 1 << LayerMask.NameToLayer("Default");
            Vector3 gravity = UnityEngine.Physics.gravity.y * Vector3.up;
            
            Calculate(startPosition, direction, impulse, timeStep, trajectoryPoints, gravity, out Vector3[] positions, out Vector3[] velocities);

            Vector3 peakPosition = startPosition;
            float highestY = startPosition.y;

            for (int i = 0; i < positions.Length - 1; i++)
            {
                if (UnityEngine.Physics.Linecast(positions[i], positions[i + 1], out _, layerMask, QueryTriggerInteraction.Ignore))
                {
                    break;
                }
                
                if (positions[i + 1].y > highestY)
                {
                    highestY = positions[i + 1].y;
                    peakPosition = positions[i + 1];
                }
            }

            return peakPosition;
        }
    }
}