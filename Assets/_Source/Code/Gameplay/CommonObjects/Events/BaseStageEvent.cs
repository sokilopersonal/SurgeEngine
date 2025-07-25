using System;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SurgeEngine.Code.Gameplay.CommonObjects.Events
{
    public abstract class BaseStageEvent : MonoBehaviour
    {
        public UnityEvent OnEventInvoked;

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
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
#endif
        }
    }
}