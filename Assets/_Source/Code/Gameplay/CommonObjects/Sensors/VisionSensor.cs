using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SurgeEngine.Code.Gameplay.CommonObjects.Sensors
{
    public class VisionSensor : MonoBehaviour
    {
        [Header("Field Of View")]
        [SerializeField] private float radius = 15f;
        [SerializeField, Range(0, 360)] private float angle = 75f;

        public float Radius => radius;
        public float Angle => angle;

        public Vector3 LastTargetPosition { get; private set; }

        [Header("Ray")] 
        [SerializeField] protected LayerMask targetMask;
        [SerializeField] protected LayerMask blockMask;

        public bool FindVisibleTargets(out Vector3 targetPosition)
        {
            if (!enabled)
            {
                targetPosition = LastTargetPosition;
                return false;
            }
            
            Collider[] hits = Physics.OverlapSphere(transform.position, radius, targetMask);

            foreach (var hit in hits)
            {
                Vector3 pos = hit.transform.position;
                Vector3 dir = (pos - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dir) < angle/2)
                {
                    float dist = Vector3.Distance(transform.position, pos);
                    bool blocked = Physics.Raycast(transform.position, dir, dist, blockMask);
                    LastTargetPosition = pos;
                    targetPosition = pos;
                    Debug.DrawLine(transform.position, pos, blocked ? Color.red : Color.green);
                    return !blocked;
                }
            }

            targetPosition = LastTargetPosition;
            return false;
        }

        private Vector3 DirectionFromAngle(float deg, bool isGlobal)
        {
            if (!isGlobal)
                deg += transform.eulerAngles.y;
            
            return new Vector3(Mathf.Sin(deg * Mathf.Deg2Rad), 0f, 
                Mathf.Cos(deg * Mathf.Deg2Rad));
        }

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            Handles.color = Color.white;
            Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, Radius);
            Vector3 viewAngleA = DirectionFromAngle(-Angle / 2, false);
            Vector3 viewAngleB = DirectionFromAngle(Angle / 2, false);
            
            Handles.DrawLine(transform.position, transform.position + viewAngleA * Radius);
            Handles.DrawLine(transform.position, transform.position + viewAngleB * Radius);
#endif
        }
    }
}