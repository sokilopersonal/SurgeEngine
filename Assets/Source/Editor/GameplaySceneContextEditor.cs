using SurgeEngine.Source.Code.Infrastructure.DI;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine.Source.Editor
{
    [CustomEditor(typeof(GameplaySceneContext))]
    public class GameplaySceneContextEditor : UnityEditor.Editor
    {
        private SerializedProperty _stageField;
        private SerializedProperty _actorPrefabField;
        private SerializedProperty _spawnPointField;
        private SerializedProperty _hudPrefabField;
        private SerializedProperty _pmScreenField;
        
        private void OnEnable()
        {
            _stageField = serializedObject.FindProperty("stage");
            _actorPrefabField = serializedObject.FindProperty("characterPrefab");
            _spawnPointField = serializedObject.FindProperty("spawnPoint");
            _hudPrefabField = serializedObject.FindProperty("hudPrefab");
            _pmScreenField = serializedObject.FindProperty("pointMarkerLoadingScreenPrefab");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_stageField);

            EditorGUILayout.PropertyField(_actorPrefabField);
            EditorGUILayout.PropertyField(_spawnPointField);
            if (_spawnPointField.objectReferenceValue != null)
            {
                var spawnSO = new SerializedObject(_spawnPointField.objectReferenceValue);
                spawnSO.Update();
                
                using (new GUILayout.VerticalScope("box"))
                {
                    var style = new GUIStyle(EditorStyles.boldLabel);
                    style.alignment = TextAnchor.MiddleCenter;
                    EditorGUILayout.LabelField("Attached Spawn", style);
                    
                    var data = spawnSO.FindProperty("startData");
                    EditorGUILayout.PropertyField(data.FindPropertyRelative("startType"));
                    EditorGUILayout.PropertyField(data.FindPropertyRelative("speed"));
                    EditorGUILayout.PropertyField(data.FindPropertyRelative("time"));
                }
                
                spawnSO.ApplyModifiedProperties();
            }
            
            EditorGUILayout.PropertyField(_hudPrefabField);
            EditorGUILayout.PropertyField(_pmScreenField);

            serializedObject.ApplyModifiedProperties();
        }
    }
}