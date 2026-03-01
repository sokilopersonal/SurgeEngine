using SurgeEngine.Source.Code.Rendering;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine.Source.Editor.GrassRendering
{
    [CustomEditor(typeof(GrassRenderer))]
    public class GrassRendererEditor : UnityEditor.Editor
    {
        private GrassRenderer _grassRenderer;
        private bool _isPainting;
        private bool _isErasing;
        private float _brushSize = 5f;
        private float _brushDensity = 2f;
        private LayerMask _paintLayer;

        private SerializedProperty _grassMeshProperty;
        private SerializedProperty _grassMaterialProperty;
        private SerializedProperty _cullingShaderProperty;
        private SerializedProperty _maxGrassCountProperty;
        private SerializedProperty _minHeightProperty;
        private SerializedProperty _maxHeightProperty;
        private SerializedProperty _minWidthProperty;
        private SerializedProperty _maxWidthProperty;
        private SerializedProperty _maxRenderDistanceProperty;
        private SerializedProperty _useRenderDistanceProperty;
        private SerializedProperty _debugCameraProperty;

        private GUIStyle _boldLabelStyle;

        private void OnEnable()
        {
            _grassRenderer = (GrassRenderer)target;

            _paintLayer = LayerMask.NameToLayer("Default");

            _grassMeshProperty = serializedObject.FindProperty("grassMesh");
            _grassMaterialProperty = serializedObject.FindProperty("grassMaterial");
            _cullingShaderProperty = serializedObject.FindProperty("cullingShader");
            _maxGrassCountProperty = serializedObject.FindProperty("maxGrassCount");
            _minHeightProperty = serializedObject.FindProperty("minHeight");
            _maxHeightProperty = serializedObject.FindProperty("maxHeight");
            _minWidthProperty = serializedObject.FindProperty("minWidth");
            _maxWidthProperty = serializedObject.FindProperty("maxWidth");
            _maxRenderDistanceProperty = serializedObject.FindProperty("maxRenderDistance");
            _useRenderDistanceProperty = serializedObject.FindProperty("useRenderDistance");
            _debugCameraProperty = serializedObject.FindProperty("debugCamera");

            _brushSize = _grassRenderer.brushSize;
            _brushDensity = _grassRenderer.brushDensity;

            SceneView.duringSceneGui += OnSceneGUIRender;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUIRender;
        }

        public override void OnInspectorGUI()
        {
            if (_boldLabelStyle == null)
            {
                _boldLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    fontStyle = FontStyle.Bold
                };
            }

            serializedObject.Update();
            EditorGUILayout.PropertyField(_grassMeshProperty);
            EditorGUILayout.PropertyField(_grassMaterialProperty);
            EditorGUILayout.PropertyField(_cullingShaderProperty);
            EditorGUILayout.PropertyField(_maxGrassCountProperty);
            EditorGUILayout.PropertyField(_minHeightProperty);
            EditorGUILayout.PropertyField(_maxHeightProperty);
            EditorGUILayout.PropertyField(_minWidthProperty);
            EditorGUILayout.PropertyField(_maxWidthProperty);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Performance Settings", _boldLabelStyle);
            EditorGUILayout.PropertyField(_useRenderDistanceProperty, new GUIContent("Use Distance Culling"));
            EditorGUILayout.PropertyField(_debugCameraProperty, new GUIContent("Debug Camera"));

            EditorGUI.BeginDisabledGroup(!_useRenderDistanceProperty.boolValue);
            EditorGUILayout.PropertyField(_maxRenderDistanceProperty);
            if (_maxRenderDistanceProperty.floatValue <= 0)
                EditorGUILayout.HelpBox("Render distance must be greater than zero.", MessageType.Warning);
            EditorGUI.EndDisabledGroup();

            if (_useRenderDistanceProperty.boolValue)
                EditorGUILayout.HelpBox("Grass will only be rendered within the specified distance from the camera, improving performance.", MessageType.Info);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Grass Painter", _boldLabelStyle);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Brush Size");
            float newSize = EditorGUILayout.Slider(_brushSize, 0.5f, 8f);
            if (newSize != _brushSize)
            {
                _brushSize = newSize;
                _grassRenderer.brushSize = newSize;
                EditorUtility.SetDirty(_grassRenderer);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Brush Density");
            float newDensity = EditorGUILayout.Slider(_brushDensity, 0.5f, 10f);
            if (newDensity != _brushDensity)
            {
                _brushDensity = newDensity;
                _grassRenderer.brushDensity = newDensity;
                EditorUtility.SetDirty(_grassRenderer);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Paint Layer");
            _paintLayer = EditorGUILayout.LayerField(_paintLayer);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = _isPainting ? Color.green : Color.white;
            if (GUILayout.Button("Paint"))
            {
                _isPainting = !_isPainting;
                _isErasing = false;
            }

            GUI.backgroundColor = _isErasing ? Color.red : Color.white;
            if (GUILayout.Button("Erase"))
            {
                _isErasing = !_isErasing;
                _isPainting = false;
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUI.BeginDisabledGroup(_grassRenderer.grassInstances.Count == 0);
            if (GUILayout.Button("Regenerate Grass"))
            {
                if (EditorUtility.DisplayDialog("Regenerate Grass",
                        "Are you sure you want to regenerate all grass instances?",
                        "Yes", "Cancel"))
                {
                    Undo.RecordObject(_grassRenderer, "Regenerate Grass");
                    _grassRenderer.RegenerateGrass();
                    EditorUtility.SetDirty(_grassRenderer);
                }
            }
            if (GUILayout.Button("Clear All Grass"))
            {
                if (EditorUtility.DisplayDialog("Clear Grass",
                        "Are you sure you want to remove all grass instances?",
                        "Yes", "Cancel"))
                {
                    Undo.RecordObject(_grassRenderer, "Clear Grass");
                    _grassRenderer.ClearGrass();
                    EditorUtility.SetDirty(_grassRenderer);
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Grass Count: {_grassRenderer.grassInstances.Count} / {_maxGrassCountProperty.intValue}");

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(_grassRenderer);
        }

        private void OnSceneGUIRender(SceneView obj)
        {
            if (!_isPainting && !_isErasing && !Event.current.alt)
                return;

            Event e = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;

            if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
            {
                if (_isPainting && e.button == 0)
                {
                    if (Physics.Raycast(ray, out hit, 500f, GetMask(), QueryTriggerInteraction.Ignore))
                    {
                        if (Vector3.Dot(hit.normal, Vector3.up) > 0.5f)
                        {
                            PaintGrass(hit.point, hit.normal);
                            e.Use();
                        }
                    }
                }
                else if (_isErasing && e.button == 0)
                {
                    if (Physics.Raycast(ray, out hit, 500f, GetMask(), QueryTriggerInteraction.Ignore))
                    {
                        EraseGrass(hit.point);
                        e.Use();
                    }
                }
            }

            if (e.shift && e.type == EventType.MouseDown && e.button == 0)
            {
                if (Physics.Raycast(ray, out hit, 500f, GetMask(), QueryTriggerInteraction.Ignore))
                {
                    if (Vector3.Dot(hit.normal, Vector3.up) > 0.5f)
                    {
                        PaintGrass(hit.point, hit.normal);
                        e.Use();
                    }
                }
            }
            else if (e.control && e.type == EventType.MouseDown && e.button == 0)
            {
                if (Physics.Raycast(ray, out hit, 500f, GetMask(), QueryTriggerInteraction.Ignore))
                {
                    EraseGrass(hit.point);
                    e.Use();
                }
            }

            if (Physics.Raycast(ray, out hit, 500f, GetMask()))
            {
                bool validSurface = Vector3.Dot(hit.normal, Vector3.up) > 0.5f;

                Color previewColor;
                if (_isPainting)
                    previewColor = validSurface ? new Color(0, 1, 0, 0.2f) : new Color(1, 0.5f, 0, 0.2f);
                else if (_isErasing)
                    previewColor = new Color(1, 0, 0, 0.2f);
                else
                    previewColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);

                Handles.color = previewColor;
                Handles.DrawSolidDisc(hit.point, hit.normal, _brushSize);

                Handles.color = _isPainting ?
                    (validSurface ? Color.green : new Color(1, 0.5f, 0)) :
                    _isErasing ? Color.red : Color.gray;

                Handles.DrawWireDisc(hit.point, hit.normal, _brushSize);

                SceneView.RepaintAll();
            }
        }

        private void PaintGrass(Vector3 center, Vector3 surfaceNormal)
        {
            Undo.RecordObject(_grassRenderer, "Paint Grass");

            int instanceCount = Mathf.RoundToInt(Mathf.Pow(_brushSize, 2) * _brushDensity * 0.2f);
            Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
            float minSpacingSqr = Mathf.Pow(_brushSize / (Mathf.Sqrt(instanceCount) + 5f), 2);
            int mask = GetMask();
            bool anyAdded = false;

            for (int i = 0; i < instanceCount; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float distance = Mathf.Sqrt(Random.value) * _brushSize;

                Vector3 flatOffset = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
                Vector3 position = center + surfaceRotation * flatOffset;

                if (!Physics.Raycast(position + surfaceNormal * 10f, -surfaceNormal, out RaycastHit hit, 20f, mask, QueryTriggerInteraction.Ignore))
                    continue;

                if (Vector3.Dot(hit.normal, Vector3.up) <= 0.5f)
                    continue;

                Vector3 spawnPos = hit.point;

                bool tooClose = false;
                foreach (var existing in _grassRenderer.grassInstances)
                {
                    if ((existing.position - spawnPos).sqrMagnitude < minSpacingSqr)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (tooClose) continue;

                _grassRenderer.grassInstances.Add(new GrassRenderer.GrassInstance
                {
                    position = spawnPos,
                    rotation = Random.Range(0f, 360f),
                    height = Random.Range(_minHeightProperty.floatValue, _maxHeightProperty.floatValue),
                    width = Random.Range(_minWidthProperty.floatValue, _maxWidthProperty.floatValue),
                    textureIndex = Random.Range(0, 4)
                });

                anyAdded = true;
            }

            if (anyAdded)
            {
                _grassRenderer.UpdateMatrices();
                EditorUtility.SetDirty(_grassRenderer);
            }
        }

        private void EraseGrass(Vector3 center)
        {
            Undo.RecordObject(_grassRenderer, "Erase Grass");
            _grassRenderer.RemoveGrassInRadius(center, _brushSize);
            EditorUtility.SetDirty(_grassRenderer);
        }

        private LayerMask GetMask() => 1 << _paintLayer;
    }
}