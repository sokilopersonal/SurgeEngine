using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine.Source.Editor.ObjectSet
{
    public class ObjectSet : EditorWindow
    {
        [Serializable]
        public class PrefabData
        {
            public string guid;
            public string category;
            [JsonIgnore] public GameObject prefab;
            public PrefabData(string guid, string category)
            {
                this.guid = guid;
                this.category = category;
            }
        }

        private readonly List<PrefabData> _prefabDataList = new List<PrefabData>();
        private Vector2 _scrollPosition;
        private const string SaveFilePath = "Assets/Source/Editor/ObjectSet/SelectedPrefabs.json";
        private string[] _categories;
        private GameObject _currentPrefabInstance;
        private bool _isPlacingPrefab;
        
        private int _selectedCategoryIndex = 0;
        private float _normalOffset = 0.1f;
        private bool _alwaysDrawPrefabName;
        
        [MenuItem("Surge Engine/Asset Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<ObjectSet>();
            window.titleContent = new GUIContent("Asset Manager");
            window.Show();
        }

        private void OnEnable()
        {
            _selectedCategoryIndex = EditorPrefs.GetInt("AssetManagerCategoryIndex", 0);
            _normalOffset = EditorPrefs.GetFloat("AssetManagerNormalOffset", 0.1f);
            _alwaysDrawPrefabName = EditorPrefs.GetBool("AssetManagerAlwaysDrawPrefabName", false);
            
            LoadAssetsList();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(15);
            var boxStyle = new GUIStyle("box")
            {
                normal = { background = Texture2D.grayTexture }
            };
            EditorGUILayout.BeginVertical(boxStyle);
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16
            };
            EditorGUILayout.LabelField("Asset Manager", titleStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);
            
            EditorGUILayout.HelpBox("How to use: " +
                                    "\n1. Choose category" +
                                    "\n2. Click on the object icon" +
                                    "\n3. Move your mouse to where you want place the object" +
                                    "\n4. Press LMB or Esc to place/cancel", MessageType.None);
            
            EditorGUILayout.Space(3);
            EditorGUILayout.HelpBox("Tip: You can drag & drop prefabs in the window to add it in the current category.", MessageType.Info);

            EditorGUILayout.BeginVertical(new GUIStyle("box")); // SETTINGS
            
            var settingsStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                padding = new RectOffset(0, 0, 0, -10)
            };
            EditorGUILayout.LabelField("Settings", settingsStyle);
            EditorGUILayout.Space(10);
            
            _categories = new[] { "All", "Common", "Enemies", "Ring Groups", "Cameras", "System", "Player System", "Thorns", "Object Physics" };
            
            _selectedCategoryIndex = EditorGUILayout.Popup("Category", _selectedCategoryIndex, _categories);
            _normalOffset = EditorGUILayout.Slider("Normal Offset", _normalOffset, 0, 0.5f);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Always Draw Prefab Name", GUILayout.Width(155));
            _alwaysDrawPrefabName = EditorGUILayout.Toggle("", _alwaysDrawPrefabName);
            EditorGUILayout.EndHorizontal();
            
            EditorPrefs.SetInt("AssetManagerCategoryIndex", _selectedCategoryIndex);
            EditorPrefs.SetFloat("AssetManagerNormalOffset", _normalOffset);
            EditorPrefs.SetBool("AssetManagerAlwaysDrawPrefabName", _alwaysDrawPrefabName);
            
            EditorGUILayout.EndVertical(); // SETTINGS END
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Prefab", GUILayout.Width(150)))
            {
                SelectPrefab();
            }
            if (GUILayout.Button("Save List", GUILayout.Width(150)))
            {
                SaveAssetsList();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Prefabs", EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            int itemsPerRow = Mathf.Max(1, (int)(position.width / 100));
            int drawnCount = 0;
            EditorGUILayout.BeginHorizontal();
            foreach (var prefabData in _prefabDataList)
            {
                if (_selectedCategoryIndex != 0 && prefabData.category != _categories[_selectedCategoryIndex])
                    continue;

                if (DrawPrefabButton(prefabData))
                {
                    ShowContextMenu(prefabData);
                }

                drawnCount++;
                if (drawnCount % itemsPerRow == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            
            var fullWindowRect = new Rect(0, 0, position.width, position.height);

            if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                && fullWindowRect.Contains(Event.current.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (Event.current.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (var obj in DragAndDrop.objectReferences)
                    {
                        if (obj is GameObject go)
                        {
                            string path = AssetDatabase.GetAssetPath(go);
                            if (PrefabUtility.GetPrefabAssetType(go) != PrefabAssetType.NotAPrefab)
                            {
                                if (_selectedCategoryIndex == 0)
                                {
                                    EditorUtility.DisplayDialog("Asset Manager",
                                        "You can't add prefab in the 'All' category. Please select something different.", "OK");
                                    continue;
                                }
                                if (!_prefabDataList.Exists(d => d.prefab == go))
                                {
                                    string guid = AssetDatabase.AssetPathToGUID(path);
                                    string category = _categories[_selectedCategoryIndex];
                                    _prefabDataList.Add(new PrefabData(guid, category) { prefab = go });
                                }
                            }
                        }
                    }
                }

                Event.current.Use();
            }
        }

        private bool DrawPrefabButton(PrefabData prefabData)
        {
            bool rightClick = false;
            EditorGUILayout.BeginVertical(GUILayout.Width(100));
            {
                Texture2D previewTexture = AssetPreview.GetAssetPreview(prefabData.prefab);

                int size = 110;
                var buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    padding = new RectOffset(1, 1, 1, 1),
                };
                var rect = GUILayoutUtility.GetRect(size, size);
                if (GUI.Button(rect, previewTexture, buttonStyle))
                {
                    if (Event.current.button == 0)
                    {
                        StartPlacingPrefab(prefabData.prefab);
                    }
                    else if (Event.current.button == 1)
                    {
                        rightClick = true;
                    }
                }
                
                GUIStyle style = new GUIStyle
                {
                    font = EditorStyles.boldFont,
                    normal = { textColor = Color.white, background = Texture2D.grayTexture },
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 10,
                    padding = new RectOffset(2, 2, 2, 2),
                    wordWrap = true
                };
                if (rect.Contains(Event.current.mousePosition) || _alwaysDrawPrefabName)
                {
                    float labelHeight = style.CalcHeight(new GUIContent(prefabData.prefab.name), rect.width);
                    GUI.Label(new Rect(rect.x, rect.y, rect.width, Mathf.Max(20, labelHeight)), prefabData.prefab.name, style);
                }
            }
            EditorGUILayout.EndVertical();
            return rightClick;
        }

        private void ShowContextMenu(PrefabData prefabData)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Move Up"), false, () => MovePrefab(prefabData, -1));
            menu.AddItem(new GUIContent("Move Down"), false, () => MovePrefab(prefabData, 1));
            menu.AddItem(new GUIContent("Move to Top"), false, () => MovePrefabToTop(prefabData));
            menu.AddItem(new GUIContent("Move to Bottom"), false, () => MovePrefabToBottom(prefabData));
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Remove"), false, () => RemovePrefab(prefabData));
            menu.ShowAsContext();
        }

        private void MovePrefab(PrefabData prefabData, int direction)
        {
            int index = _prefabDataList.IndexOf(prefabData);
            int newIndex = Mathf.Clamp(index + direction, 0, _prefabDataList.Count - 1);
            if (index != newIndex)
            {
                _prefabDataList.RemoveAt(index);
                _prefabDataList.Insert(newIndex, prefabData);
            }
        }

        private void MovePrefabToTop(PrefabData prefabData)
        {
            _prefabDataList.Remove(prefabData);
            _prefabDataList.Insert(0, prefabData);
        }

        private void MovePrefabToBottom(PrefabData prefabData)
        {
            _prefabDataList.Remove(prefabData);
            _prefabDataList.Add(prefabData);
        }

        private void SelectPrefab()
        {
            if (_selectedCategoryIndex == 0)
            {
                EditorUtility.DisplayDialog("Asset Manager",
                    "You can't add prefab in the 'All' category. Please select something different.", "OK");
                return;
            }
            
            string path = EditorUtility.OpenFilePanel("Select Prefab", "", "prefab");
            if (!string.IsNullOrEmpty(path))
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab != null && !_prefabDataList.Exists(data => data.prefab == prefab))
                {
                    string category = _categories[_selectedCategoryIndex];
                    _prefabDataList.Add(new PrefabData(path, category) { prefab = prefab } );
                }
            }
        }

        private void RemovePrefab(PrefabData prefabData)
        {
            _prefabDataList.Remove(prefabData);
        }

        private void SaveAssetsList()
        {
            var jsonList = new List<PrefabData>();
            foreach (var data in _prefabDataList)
            {
                var path = AssetDatabase.GetAssetPath(data.prefab);
                var guid = AssetDatabase.AssetPathToGUID(path);
                jsonList.Add(new PrefabData(guid, data.category) { prefab = data.prefab });
            }
            var json = JsonConvert.SerializeObject(jsonList, Formatting.Indented);
            File.WriteAllText(SaveFilePath, json);
            AssetDatabase.Refresh();
        }

        private void LoadAssetsList()
        {
            _prefabDataList.Clear();
            if (!File.Exists(SaveFilePath)) return;
            var json = File.ReadAllText(SaveFilePath);
            var jsonList = JsonConvert.DeserializeObject<List<PrefabData>>(json);
            foreach (var data in jsonList)
            {
                var path = AssetDatabase.GUIDToAssetPath(data.guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                    _prefabDataList.Add(new PrefabData(data.guid, data.category) { prefab = prefab });
            }
        }

        private void StartPlacingPrefab(GameObject prefab)
        {
            if (_currentPrefabInstance != null)
            {
                DestroyImmediate(_currentPrefabInstance);
            }

            _currentPrefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            GameObject parent = GameObject.FindGameObjectWithTag("SetData");
            if (parent != null)
            {
                _currentPrefabInstance.transform.SetParent(parent.transform, true);
            }
            
            _isPlacingPrefab = true;
            SceneView.duringSceneGui += DuringSceneGUI;
            FocusWindowIfItsOpen<SceneView>();
        }
        
        private void PlacePrefab()
        {
            if (_currentPrefabInstance != null)
            {
                Undo.RegisterCreatedObjectUndo(_currentPrefabInstance, "Place Prefab");
                Selection.activeObject = _currentPrefabInstance;
                _currentPrefabInstance = null;
                _isPlacingPrefab = false;
                SceneView.duringSceneGui -= DuringSceneGUI;
            }
        }
        
        private void DuringSceneGUI(SceneView sceneView)
        {
            if (_isPlacingPrefab && _currentPrefabInstance != null)
            {
                Event e = Event.current;
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                
                Collider[] colliders = _currentPrefabInstance.GetComponentsInChildren<Collider>();
                foreach (var collider in colliders)
                {
                    collider.enabled = false;
                }
                
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Default")))
                {
                    _currentPrefabInstance.transform.position = hit.point + hit.normal * _normalOffset;
                }

                foreach (var collider in colliders)
                {
                    collider.enabled = true;
                }
                
                Handles.color = Color.green;
                Handles.SphereHandleCap(0, hit.point, Quaternion.identity, 0.25f, EventType.Repaint);

                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    PlacePrefab();
                    e.Use();
                }

                if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
                {
                    DestroyImmediate(_currentPrefabInstance);
                    e.Use();
                }

                SceneView.RepaintAll();
            }
        }
    }
}
