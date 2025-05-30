using SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine._Source.Editor
{
    [CustomEditor(typeof(ObjCameraNormal))]
    public class CameraNormalEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var dataProp = serializedObject.FindProperty("data");
            
            EditorGUILayout.HelpBox("Pans the player's camera:\n- Camera changes its distance, vertical offset and base field of view", MessageType.Info);
        
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("easeTimeEnter"), new GUIContent("Ease Time Enter"));
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("easeTimeExit"), new GUIContent("Ease Time Exit"));
            
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("distance"), new GUIContent("Distance"));
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("yOffset"), new GUIContent("Vertical Offset"));
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("fov"), new GUIContent("Field Of View"));
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}