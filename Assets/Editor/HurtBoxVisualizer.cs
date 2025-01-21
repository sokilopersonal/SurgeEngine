using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine.Editor
{
    public class HurtBoxVisualizer : EditorWindow
    {
        private HurtBoxVisualizerMode _mode;
        private Transform _transform;
        private Vector3 _offset;
        private Vector3 _size = Vector3.one;
        
        [MenuItem("Surge Engine/Hurt Box Visualizer")]
        private static void ShowWindow()
        {
            var window = GetWindow<HurtBoxVisualizer>();
            window.titleContent = new GUIContent("Hurt Box Visualizer");
            window.minSize = new Vector2(300, 180);
            window.Show();
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Assign transform and size", EditorStyles.boldLabel);
            
            _mode = (HurtBoxVisualizerMode)EditorGUILayout.EnumPopup("Mode", _mode);
            
            _transform = EditorGUILayout.ObjectField("Transform", _transform, typeof(Transform), true) as Transform;
            if (_mode == HurtBoxVisualizerMode.PositionOffset)
            {
                _offset = EditorGUILayout.Vector3Field("Offset", _offset);
            }
            
            _size = EditorGUILayout.Vector3Field("Size", _size);

            if (GUILayout.Button("Copy Offset"))
            {
                string vector = $"new Vector3({_offset.x.ToString(CultureInfo.InvariantCulture)}f, {_offset.y.ToString(CultureInfo.InvariantCulture)}f, {_offset.z.ToString(CultureInfo.InvariantCulture)}f)";
                EditorGUIUtility.systemCopyBuffer = vector;
            }
            if (GUILayout.Button("Copy Size"))
            {
                string vector = $"new Vector3({_size.x.ToString(CultureInfo.InvariantCulture)}f, {_size.y.ToString(CultureInfo.InvariantCulture)}f, {_size.z.ToString(CultureInfo.InvariantCulture)}f)";
                EditorGUIUtility.systemCopyBuffer = vector;
            }
        }

        private void OnSceneGUI(SceneView obj)
        {
            if (_transform != null)
            {
                Handles.color = Color.red;
                Handles.matrix = _transform.localToWorldMatrix;
                if (_mode == HurtBoxVisualizerMode.Attached)
                {
                    Handles.DrawWireCube(Vector3.zero, _size);
                }
                else
                {
                    Handles.DrawWireCube(_offset, _size);
                }
            }
        }
    }

    public enum HurtBoxVisualizerMode
    {
        Attached,
        PositionOffset
    }
}