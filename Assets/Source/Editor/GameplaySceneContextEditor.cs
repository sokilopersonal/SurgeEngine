using SurgeEngine.Source.Code.Infrastructure.DI;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine.Source.Editor
{
    [CustomEditor(typeof(GameplaySceneContext))]
    public class GameplaySceneContextEditor : UnityEditor.Editor
    {
        private SerializedProperty _spawnPointField;
        
        private void OnEnable()
        {
            _spawnPointField = serializedObject.FindProperty("spawnPoint");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();
            SerializedProperty iterator = serializedObject.GetIterator();

            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                if (iterator.propertyPath == "spawnPoint")
                {
                    var spawnSo = new SerializedObject(_spawnPointField.objectReferenceValue);
                    spawnSo.Update();
                    
                    EditorGUILayout.PropertyField(_spawnPointField);
                    
                    using (new GUILayout.VerticalScope("box"))
                    {
                        var style = new GUIStyle(EditorStyles.boldLabel);
                        style.alignment = TextAnchor.MiddleCenter;
                        EditorGUILayout.LabelField("Attached Spawn", style);
                    
                        var data = spawnSo.FindProperty("startData");
                        EditorGUILayout.PropertyField(data.FindPropertyRelative("startType"));
                        EditorGUILayout.PropertyField(data.FindPropertyRelative("speed"));
                        EditorGUILayout.PropertyField(data.FindPropertyRelative("time"));
                    }

                    spawnSo.ApplyModifiedProperties();
                    
                    continue;
                }

                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}