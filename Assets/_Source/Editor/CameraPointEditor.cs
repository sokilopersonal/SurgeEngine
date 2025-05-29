using SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine._Source.Editor
{
    [CustomEditor(typeof(ObjCameraPoint))]
    public class CameraPointEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var dataProp = serializedObject.FindProperty("data");
            
            EditorGUILayout.HelpBox("Pans the player's camera:\n- Camera moves with the player at a set distance & offset\n- Looks in the object's forward direction + offset", MessageType.Info);
        
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("easeTimeEnter"), new GUIContent("Ease Time Enter"));
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("easeTimeExit"), new GUIContent("Ease Time Exit"));
            
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("distance"), new GUIContent("Distance"));
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("yOffset"), new GUIContent("Vertical Offset"));
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("offset"), new GUIContent("Position Offset"));
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("localLookOffset"), new GUIContent("Look Offset"));
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("allowRotation"), new GUIContent("Allow Rotation?"));
            EditorGUILayout.PropertyField(dataProp.FindPropertyRelative("fov"), new GUIContent("Field Of View"));
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}