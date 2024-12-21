using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class WideSpring : Spring
    {
        protected override void Awake()
        {
            direction = Vector3.up;
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            Vector3 startPosition = transform.position + Vector3.up * yOffset;
            Vector3 dir = Vector3.up;

            if (keepVelocity > 0f)
            {
                Vector3 newStartPosition = startPosition + dir * keepVelocity * speed;
                Debug.DrawLine(startPosition, newStartPosition, Color.red);
                startPosition = newStartPosition;
            }
            
            Debug.DrawLine(startPosition, startPosition + dir * speed / 2, Color.green);
        }
    }
}