using UnityEditor;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects
{
    public abstract class ObjCameraBase : MonoBehaviour
    {
        public abstract void SetPan();

        public abstract void RemovePan();

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            float multiplier = 2.5f;
            
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.up * multiplier);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.forward * multiplier);
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.right * multiplier);
            
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            string tName = GetType().Name;
            float distance = Vector3.Distance(transform.position, Camera.current.transform.position);
            if (distance < 35f)
            {
                Handles.Label(transform.position + Vector3.up * 0.75f, tName, style);
            }
#endif
        }
    }
}