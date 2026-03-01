using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine.Source.Editor.ObjectsEditor
{
    [CustomEditor(typeof(MykonosFloor))]
    public class MykonosFloorEditor : UnityEditor.Editor
    {
        // PingPong fields
        private SerializedProperty _amplitude;
        private SerializedProperty _cycle;
        private SerializedProperty _phase;
        private SerializedProperty _pingPongDebug;

        // Falling fields
        private SerializedProperty _gravity;
        private SerializedProperty _onFloorTime;
        private SerializedProperty _resetTime;
        private SerializedProperty _model;
        private SerializedProperty _fallStepSound;

        // Shared
        private SerializedProperty _moveType;

        private static readonly Color HeaderColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        private static readonly Color AccentColor = new Color(0.2f, 0.6f, 1f, 1f);

        private void OnEnable()
        {
            _moveType = serializedObject.FindProperty("moveType");
            _amplitude = serializedObject.FindProperty("amplitude");
            _cycle = serializedObject.FindProperty("cycle");
            _phase = serializedObject.FindProperty("phase");
            _pingPongDebug = serializedObject.FindProperty("pingPongDebug");
            _gravity = serializedObject.FindProperty("gravity");
            _onFloorTime = serializedObject.FindProperty("onFloorTime");
            _resetTime = serializedObject.FindProperty("resetTime");
            _model = serializedObject.FindProperty("model");
            _fallStepSound = serializedObject.FindProperty("fallStepSound");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawCustomHeader();
            EditorGUILayout.Space(4);

            DrawSection("Move Type", () =>
            {
                EditorGUILayout.PropertyField(_moveType);
            });

            EditorGUILayout.Space(4);

            var type = (MykonosFloorType)_moveType.enumValueIndex;

            switch (type)
            {
                case MykonosFloorType.Stationary:
                    break;
                
                case MykonosFloorType.PingPong:
                    DrawSection("Ping Pong Settings", () =>
                    {
                        EditorGUILayout.PropertyField(_amplitude);
                        EditorGUILayout.PropertyField(_cycle);
                        EditorGUILayout.PropertyField(_phase);
                        EditorGUILayout.PropertyField(_pingPongDebug);
                    });
                    break;

                case MykonosFloorType.Falling:
                    DrawSection("Falling Settings", () =>
                    {
                        EditorGUILayout.PropertyField(_gravity);
                        EditorGUILayout.PropertyField(_onFloorTime, new GUIContent("On Floor Time"));
                        EditorGUILayout.PropertyField(_resetTime, new GUIContent("Reset Time"));
                        EditorGUILayout.PropertyField(_model);
                        EditorGUILayout.PropertyField(_fallStepSound, new GUIContent("Fall Step Sound"));
                    });
                    break;

                default:
                    DrawInfoBox($"⚠  Move type '{type}' is not implemented yet.");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCustomHeader()
        {
            var rect = EditorGUILayout.GetControlRect(false, 32);
            EditorGUI.DrawRect(rect, HeaderColor);

            var accentRect = new Rect(rect.x, rect.y, 4, rect.height);
            EditorGUI.DrawRect(accentRect, AccentColor);

            var labelRect = new Rect(rect.x + 12, rect.y, rect.width - 12, rect.height);
            EditorGUI.LabelField(labelRect, "Mykonos Floor", new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Color.white }
            });
        }

        private void DrawSection(string title, System.Action content)
        {
            var headerRect = EditorGUILayout.GetControlRect(false, 22);
            EditorGUI.DrawRect(headerRect, new Color(0.22f, 0.22f, 0.22f, 1f));

            var accentRect = new Rect(headerRect.x, headerRect.y, 3, headerRect.height);
            EditorGUI.DrawRect(accentRect, AccentColor);

            EditorGUI.LabelField(
                new Rect(headerRect.x + 10, headerRect.y, headerRect.width - 10, headerRect.height),
                title,
                new GUIStyle(EditorStyles.miniLabel)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 11,
                    normal = { textColor = new Color(0.85f, 0.85f, 0.85f) }
                }
            );

            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
            EditorGUILayout.Space(2);
            content?.Invoke();
            EditorGUILayout.Space(2);
            EditorGUILayout.EndVertical();
        }

        private void DrawInfoBox(string message)
        {
            var style = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 11,
                padding = new RectOffset(10, 10, 8, 8)
            };
            EditorGUILayout.LabelField(message, style);
        }
    }
}
