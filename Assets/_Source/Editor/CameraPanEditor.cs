using SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine._Source.Editor
{
    [CustomEditor(typeof(ObjCameraPan))]
    public class CameraPanEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Pans the player's camera:\n- Stationary camera\n- Looks at the player", MessageType.Info);
        
            var dataProp = serializedObject.FindProperty("data");
        
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("easeTimeEnter"), new GUIContent("Ease Time Enter"));
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("easeTimeExit"), new GUIContent("Ease Time Exit"));
            
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("allowRotation"), new GUIContent("Allow Rotation?"));
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("fov"), new GUIContent("Field Of View"));
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}
