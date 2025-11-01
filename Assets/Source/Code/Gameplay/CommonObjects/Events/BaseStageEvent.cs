using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Events
{
    public abstract class BaseStageEvent : MonoBehaviour
    {
        public UnityEvent OnEventInvoked;

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var view = SceneView.lastActiveSceneView;
            if (view != null)
            {
                var cam = SceneView.lastActiveSceneView.camera;
                if (cam != null)
                {
                    Handles.color = Color.white;
                    float distance = Vector3.Distance(transform.position, cam.transform.position);

                    if (distance < 40f)
                    {
                        var style = new GUIStyle(EditorStyles.boldLabel);
                        style.alignment = TextAnchor.MiddleCenter;
                        style.fontSize = 16;
                        Handles.Label(transform.position, GetType().Name, style);
                
                        Gizmos.DrawWireSphere(transform.position, 0.5f);
                    }
                }
            }
#endif
        }
    }
}