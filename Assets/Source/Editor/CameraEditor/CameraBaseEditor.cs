using System.Reflection;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine.Source.Editor.CameraEditor
{
    [CustomEditor(typeof(ObjCameraBase), true)]
    public class CameraBaseEditor : UnityEditor.Editor
    {
        private bool isPicking;
        private SerializedProperty targetProp;

        private void OnEnable()
        {
            targetProp = serializedObject.FindProperty("target");
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            if (isPicking)
                SceneView.duringSceneGui -= OnSceneGUI;
            Selection.selectionChanged -= OnSelectionChanged;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if (GUILayout.Button(isPicking ? "Cancel" : "Attach Camera to Volume"))
            {
                isPicking = !isPicking;
                if (isPicking)
                    SceneView.duringSceneGui += OnSceneGUI;
                else
                    SceneView.duringSceneGui -= OnSceneGUI;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI(SceneView sv)
        {
            var e = Event.current;
            var cam = (ObjCameraBase)target;
            if (e.type is EventType.Repaint or EventType.MouseMove || e.type == EventType.MouseDown)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Default")))
                {
                    Handles.color = Color.yellow;
                    Handles.DrawLine(cam.transform.position, hit.point);
                    Handles.DrawWireDisc(hit.point, hit.normal, 0.2f);
                    SceneView.RepaintAll();
                }
            }
            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                var go = HandleUtility.PickGameObject(e.mousePosition, false);
                if (go != null)
                {
                    var comp = go.GetComponent<ChangeVolumeCamera>();
                    if (comp != null)
                    {
                        var f = typeof(ChangeVolumeCamera).GetField("target", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        if (f != null)
                        {
                            Undo.RecordObject(comp, "Assign Camera Target");
                            f.SetValue(comp, cam);
                            EditorUtility.SetDirty(comp);
                        }
                    }
                }
                isPicking = false;
                SceneView.duringSceneGui -= OnSceneGUI;
                e.Use();
            }
        }

        private void OnSelectionChanged()
        {
            if (targetProp == null) return;
            
            serializedObject.Update();
            
            if (targetProp.objectReferenceValue != null)
            {
                var sel = Selection.objects;
                
                bool found = false;
                foreach (var o in sel)
                {
                    if (o == targetProp.objectReferenceValue)
                    {
                        found = true;
                        break;
                    }
                }
                
                if (!found)
                {
                    targetProp.objectReferenceValue = null;
                    serializedObject.ApplyModifiedProperties();
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
