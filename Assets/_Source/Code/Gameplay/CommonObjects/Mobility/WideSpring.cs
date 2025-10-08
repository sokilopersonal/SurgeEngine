using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public class WideSpring : Spring
    {
        public override Vector3 Direction => Vector3.up;

        private void OnDrawGizmosSelected()
        {
            Vector3 start = transform.position + Vector3.up * 0.5f;

            if (keepVelocityDistance > 0f)
            {
                Gizmos.color = Color.red;
                var dir = Vector3.up;
                var newStartPos = start + dir * keepVelocityDistance;
                Gizmos.DrawLine(start, newStartPos);
                start = newStartPos;
            }

            float v0 = speed;
            float g = Physics.gravity.y;
            Gizmos.color = Color.green;

            Vector3 prev = start;
            float dt = 0.1f;
            for (float t = dt; t < 2 * v0 / -g; t += dt)
            {
                float y = v0 * t + 0.5f * g * t * t;
                Vector3 next = start + Vector3.up * y;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
    }
}