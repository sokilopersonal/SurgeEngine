using System.Reflection;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.CameraObjects;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace SurgeEngine._Source.Editor.CameraEditor
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
            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                var go = HandleUtility.PickGameObject(e.mousePosition, false);
                if (go != null)
                {
                    var comp = go.GetComponent<ChangeCameraVolume>();
                    if (comp != null)
                    {
                        var editorTarget = (ObjCameraBase)target;
                        var f = typeof(ChangeCameraVolume).GetField("target", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        if (f != null)
                        {
                            Undo.RecordObject(comp, "Assign Camera Target");
                            f.SetValue(comp, editorTarget);
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
