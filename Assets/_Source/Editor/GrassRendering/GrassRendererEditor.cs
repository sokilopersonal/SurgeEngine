using SurgeEngine.Code.Rendering;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine._Source.Editor.GrassRendering
{
    [CustomEditor(typeof(GrassRenderer))]
    public class GrassRendererEditor : UnityEditor.Editor
    {
        private GrassRenderer _grassRenderer;
        private bool _isPainting;
        private bool _isErasing;
        private bool _isRandomizing;
        private float _brushSize = 5f;
        private float _brushDensity = 1f;
        private LayerMask _paintLayer = 0;
    
        private SerializedProperty _grassMeshProperty;
        private SerializedProperty _grassMaterialProperty;
        private SerializedProperty _maxGrassCountProperty;
        private SerializedProperty _minHeightProperty;
        private SerializedProperty _maxHeightProperty;
        private SerializedProperty _minWidthProperty;
        private SerializedProperty _maxWidthProperty;
        private SerializedProperty _maxRenderDistanceProperty;
        private SerializedProperty _useRenderDistanceProperty;
    
        private GUIStyle _boldLabelStyle;

        private void OnEnable()
        {
            _grassRenderer = (GrassRenderer)target;
            
            _paintLayer = 1 << _grassRenderer.gameObject.layer;
        
            _grassMeshProperty = serializedObject.FindProperty("grassMesh");
            _grassMaterialProperty = serializedObject.FindProperty("grassMaterial");
            _maxGrassCountProperty = serializedObject.FindProperty("maxGrassCount");
            _minHeightProperty = serializedObject.FindProperty("minHeight");
            _maxHeightProperty = serializedObject.FindProperty("maxHeight");
            _minWidthProperty = serializedObject.FindProperty("minWidth");
            _maxWidthProperty = serializedObject.FindProperty("maxWidth");
            _maxRenderDistanceProperty = serializedObject.FindProperty("maxRenderDistance");
            _useRenderDistanceProperty = serializedObject.FindProperty("useRenderDistance");
        
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
        
            EditorGUILayout.LabelField("Grass Renderer", _boldLabelStyle);
            EditorGUILayout.PropertyField(_grassMeshProperty);
            EditorGUILayout.PropertyField(_grassMaterialProperty);
            EditorGUILayout.PropertyField(_maxGrassCountProperty);
        
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Grass Properties", _boldLabelStyle);
            EditorGUILayout.PropertyField(_minHeightProperty);
            EditorGUILayout.PropertyField(_maxHeightProperty);
            EditorGUILayout.PropertyField(_minWidthProperty);
            EditorGUILayout.PropertyField(_maxWidthProperty);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Performance Settings", _boldLabelStyle);
            EditorGUILayout.PropertyField(_useRenderDistanceProperty, new GUIContent("Use Distance Culling"));
            
            EditorGUI.BeginDisabledGroup(!_useRenderDistanceProperty.boolValue);
            EditorGUILayout.PropertyField(_maxRenderDistanceProperty);
            if (_maxRenderDistanceProperty.floatValue <= 0)
            {
                EditorGUILayout.HelpBox("Render distance must be greater than zero.", MessageType.Warning);
            }
            EditorGUI.EndDisabledGroup();
            
            if (_useRenderDistanceProperty.boolValue)
            {
                EditorGUILayout.HelpBox("Grass will only be rendered within the specified distance from the camera, improving performance.", MessageType.Info);
            }
        
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Grass Painter", _boldLabelStyle);
        
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Brush Size");
            _brushSize = EditorGUILayout.Slider(_brushSize, 0.5f, 20f);
            EditorGUILayout.EndHorizontal();
        
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Brush Density");
            _brushDensity = EditorGUILayout.Slider(_brushDensity, 0.1f, 5f);
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
                _isRandomizing = false;
            }
            
            GUI.backgroundColor = _isRandomizing ? Color.yellow : Color.white;
            if (GUILayout.Button("Randomize"))
            {
                _isRandomizing = !_isRandomizing;
                _isPainting = false;
                _isErasing = false;
            }
                    
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(_grassRenderer.grassInstances.Count == 0);
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
            EditorGUILayout.EndHorizontal();
        
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
        
            if (GUILayout.Button("Save Grass"))
            {
                string path = EditorUtility.SaveFilePanel(
                    "Save Grass Data",
                    Application.dataPath,
                    "GrassData",
                    "grass"
                );
            
                if (!string.IsNullOrEmpty(path))
                {
                    string fileName = System.IO.Path.GetFileName(path);
                    GrassData data = new GrassData(_grassRenderer.grassInstances);
                    data.SaveToFile(fileName);
                }
            }
        
            if (GUILayout.Button("Load Grass"))
            {
                string path = EditorUtility.OpenFilePanel(
                    "Load Grass Data",
                    Application.persistentDataPath,
                    "grass"
                );
            
                if (!string.IsNullOrEmpty(path))
                {
                    string fileName = System.IO.Path.GetFileName(path);
                    GrassData data = GrassData.LoadFromFile(fileName);
                
                    if (data != null)
                    {
                        Undo.RecordObject(_grassRenderer, "Load Grass");
                        _grassRenderer.grassInstances = data.ToGrassInstances();
                        _grassRenderer.UpdateMatrices();
                        EditorUtility.SetDirty(_grassRenderer);
                    }
                }
            }
        
            EditorGUILayout.EndHorizontal();
        
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Grass Count: {_grassRenderer.grassInstances.Count} / {_maxGrassCountProperty.intValue}");
        
            serializedObject.ApplyModifiedProperties();
        
            if (GUI.changed)
            {
                EditorUtility.SetDirty(_grassRenderer);
            }
        }
    
        private void OnSceneGUIRender(SceneView obj)
        {
            if (!_isPainting && !_isErasing && !_isRandomizing && !Event.current.alt)
                return;
            
            Event e = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;

            if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
            {
                if (_isPainting && e.button == 0)
                {
                    if (Physics.Raycast(ray, out hit, 500f, _paintLayer, QueryTriggerInteraction.Ignore))
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
                    if (Physics.Raycast(ray, out hit, 500f, _paintLayer, QueryTriggerInteraction.Ignore))
                    {
                        EraseGrass(hit.point);
                        e.Use();
                    }
                }
                else if ((_isRandomizing || e.alt) && e.button == 0)
                {
                    if (Physics.Raycast(ray, out hit, 500f, _paintLayer, QueryTriggerInteraction.Ignore))
                    {
                        RandomizeGrass(hit.point);
                        e.Use();
                    }
                }
            }

            if (e.shift && e.type == EventType.MouseDown && e.button == 0)
            {
                if (Physics.Raycast(ray, out hit, 500f, _paintLayer, QueryTriggerInteraction.Ignore))
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
                if (Physics.Raycast(ray, out hit, 500f, _paintLayer, QueryTriggerInteraction.Ignore))
                {
                    EraseGrass(hit.point);
                    e.Use();
                }
            }
            else if (e.alt && e.type == EventType.MouseDown && e.button == 0)
            {
                if (Physics.Raycast(ray, out hit, 500f, _paintLayer, QueryTriggerInteraction.Ignore))
                {
                    RandomizeGrass(hit.point);
                    e.Use();
                }
            }

            if (Physics.Raycast(ray, out hit, 500f, _paintLayer, QueryTriggerInteraction.Ignore))
            {
                Color previewColor;
                bool validSurface = Vector3.Dot(hit.normal, Vector3.up) > 0.5f;
                
                if (_isPainting)
                {
                    previewColor = validSurface ? 
                        new Color(0, 1, 0, 0.2f) : new Color(1, 0.5f, 0, 0.2f);
                }
                else if (_isErasing)
                {
                    previewColor = new Color(1, 0, 0, 0.2f);
                }
                else if (_isRandomizing || e.alt)
                {
                    previewColor = new Color(1, 1, 0, 0.2f);
                }
                else
                {
                    previewColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
                }
                    
                Handles.color = previewColor;
                Handles.DrawSolidDisc(hit.point, hit.normal, _brushSize);

                Handles.color = _isPainting ? 
                    (validSurface ? Color.green : new Color(1, 0.5f, 0)) : 
                    (_isErasing ? Color.red : (_isRandomizing || e.alt ? Color.yellow : Color.gray));
                    
                Handles.DrawWireDisc(hit.point, hit.normal, _brushSize);
            
                SceneView.RepaintAll();
            }
        }
    
        private void PaintGrass(Vector3 center, Vector3 surfaceNormal)
        {
            Undo.RecordObject(_grassRenderer, "Paint Grass");

            int instanceCount = Mathf.RoundToInt(Mathf.Pow(_brushSize, 2) * _brushDensity * 0.2f);

            Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
        
            for (int i = 0; i < instanceCount; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float distance = Random.Range(0f, _brushSize);

                Vector3 flatOffset = new Vector3(
                    Mathf.Cos(angle) * distance,
                    0f,
                    Mathf.Sin(angle) * distance
                );

                Vector3 alignedOffset = surfaceRotation * flatOffset;
            
                Vector3 position = center + alignedOffset;

                RaycastHit hit;
                Vector3 rayStart = position + surfaceNormal * 10f;
                Vector3 rayDirection = -surfaceNormal;
                
                if (Physics.Raycast(rayStart, rayDirection, out hit, 20f, _paintLayer, QueryTriggerInteraction.Ignore))
                {
                    if (Vector3.Dot(hit.normal, Vector3.up) > 0.5f)
                    {
                        position = hit.point;
                        _grassRenderer.AddGrassInstance(position);
                    }
                }
            }
        
            EditorUtility.SetDirty(_grassRenderer);
        }
    
        private void EraseGrass(Vector3 center)
        {
            Undo.RecordObject(_grassRenderer, "Erase Grass");
            _grassRenderer.RemoveGrassInRadius(center, _brushSize);
            EditorUtility.SetDirty(_grassRenderer);
        }
        
        private void RandomizeGrass(Vector3 center)
        {
            Undo.RecordObject(_grassRenderer, "Randomize Grass");
            _grassRenderer.RandomizeGrassInRadius(center, _brushSize);
            EditorUtility.SetDirty(_grassRenderer);
        }
    }
}
