using SurgeEngine.Code.Gameplay.CommonObjects;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine._Source.Editor
{
    [CustomEditor(typeof(CharacterSpawn))]
    public class CharacterSpawnEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var startData = serializedObject.FindProperty("startData");
            
            EditorGUILayout.PropertyField(startData.FindPropertyRelative("startType"), new GUIContent("Start Type"));
            EditorGUILayout.PropertyField(startData.FindPropertyRelative("speed"), new GUIContent("Speed"));
            EditorGUILayout.PropertyField(startData.FindPropertyRelative("time"), new GUIContent("Time"));
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}