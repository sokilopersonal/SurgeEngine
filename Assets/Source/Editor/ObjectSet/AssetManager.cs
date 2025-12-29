using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine.Source.Editor.ObjectSet
{
    public class AssetManagerDatabase : ScriptableObject
    {
        [Serializable]
        public class Category
        {
            public string name;
            public List<string> prefabGUIDs = new List<string>();
        }
        
        [Serializable]
        public class PrefabData
        {
            public string guid;
            public string customName;
            [JsonIgnore] public GameObject prefab;
        }
        
        public List<Category> categories = new List<Category>();
        public List<PrefabData> prefabs = new List<PrefabData>();
        
        public void RenameCategory(string oldName, string newName)
        {
            var category = categories.FirstOrDefault(c => c.name == oldName);
            if (category != null)
            {
                category.name = newName;
                EditorUtility.SetDirty(this);
            }
        }
        
        public void AddPrefabToCategory(string guid, string categoryName)
        {
            var prefabData = prefabs.FirstOrDefault(p => p.guid == guid);
            if (prefabData == null)
            {
                prefabData = new PrefabData { guid = guid };
                prefabs.Add(prefabData);
            }
            
            var category = categories.FirstOrDefault(c => c.name == categoryName);
            if (category != null && !category.prefabGUIDs.Contains(guid))
            {
                category.prefabGUIDs.Add(guid);
                EditorUtility.SetDirty(this);
            }
        }
        
        public void RemovePrefabFromCategory(string guid, string categoryName)
        {
            var category = categories.FirstOrDefault(c => c.name == categoryName);
            if (category != null)
            {
                category.prefabGUIDs.Remove(guid);
                EditorUtility.SetDirty(this);
            }
        }
        
        public void RemovePrefab(string guid)
        {
            prefabs.RemoveAll(p => p.guid == guid);
            foreach (var category in categories)
            {
                category.prefabGUIDs.Remove(guid);
            }
            EditorUtility.SetDirty(this);
        }
    }

    public class AssetManager : EditorWindow
    {
        private AssetManagerDatabase _database;
        private Vector2 _scrollPosition;
        private GameObject _currentPrefabInstance;
        private bool _isPlacingPrefab;
        
        private int _selectedCategoryIndex = 0;
        private float _normalOffset = 0.1f;
        private string _searchString = "";
        
        private const string DatabasePath = "Assets/Source/Editor/ObjectSet/AssetManagerDatabase.asset";
        private const string LastCategoryKey = "AssetManagerCategoryIndex";
        private const string NormalOffsetKey = "AssetManagerNormalOffset";
        
        [MenuItem("Surge Engine/Asset Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<AssetManager>();
            window.titleContent = new GUIContent("Asset Manager", EditorGUIUtility.IconContent("d_Package Manager").image);
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        private void OnEnable()
        {
            _selectedCategoryIndex = EditorPrefs.GetInt(LastCategoryKey, 0);
            _normalOffset = EditorPrefs.GetFloat(NormalOffsetKey, 0.1f);
            autoRepaintOnSceneChange = true;

            LoadDatabase();
        }
        
        private void OnDisable()
        {
            SaveDatabase();
        }
        
        private void LoadDatabase()
        {
            _database = AssetDatabase.LoadAssetAtPath<AssetManagerDatabase>(DatabasePath);
            if (_database == null)
            {
                _database = CreateInstance<AssetManagerDatabase>();
                _database.categories.AddRange(new[]
                {
                    new AssetManagerDatabase.Category { name = "Common" },
                    new AssetManagerDatabase.Category { name = "Enemies" },
                    new AssetManagerDatabase.Category { name = "Ring Groups" },
                    new AssetManagerDatabase.Category { name = "Cameras" },
                    new AssetManagerDatabase.Category { name = "System" },
                    new AssetManagerDatabase.Category { name = "Player System" },
                    new AssetManagerDatabase.Category { name = "Thorns" },
                    new AssetManagerDatabase.Category { name = "Object Physics" }
                });
                
                AssetDatabase.CreateAsset(_database, DatabasePath);
                AssetDatabase.SaveAssets();
            }
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawInstructions();
            DrawSettings();
            DrawControls();
            DrawPrefabGrid();
            HandleDragAndDrop();
        }
        
        private void DrawHeader()
        {
            EditorGUILayout.Space(10);
            var headerStyle = new GUIStyle("IN BigTitle")
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(0, 0, 10, 10)
            };
            
            var headerRect = EditorGUILayout.BeginVertical(headerStyle);
            EditorGUILayout.LabelField("Asset Manager", headerStyle);
            EditorGUILayout.EndVertical();
            
            if (GUI.Button(new Rect(headerRect.xMax - 25, headerRect.y + 5, 20, 20), 
                new GUIContent(EditorGUIUtility.IconContent("pane options").image, "Settings")))
            {
                ShowSettingsMenu();
            }
        }
        
        private void DrawInstructions()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox(
                "How to use:\n" +
                "• Select category or search\n" +
                "• Click on prefab icon to place\n" +
                "• Move mouse in scene and click to place\n" +
                "• Press Esc to cancel\n" +
                "• Changes saved automatically", MessageType.Info);
        }
        
        private void DrawSettings()
        {
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("Quick Settings", 
                new GUIStyle(EditorStyles.boldLabel) { fontSize = 14 });
            
            EditorGUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(EditorGUIUtility.IconContent("Search Icon").image), GUILayout.Width(20));
            _searchString = EditorGUILayout.TextField(_searchString, GUI.skin.FindStyle("SearchTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("SearchCancelButton")))
            {
                _searchString = "";
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();
            var categories = new[] { "All" }.Concat(_database.categories.Select(c => c.name)).ToArray();
            _selectedCategoryIndex = EditorGUILayout.Popup("Category", _selectedCategoryIndex, categories);
            
            if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("editicon.sml").image, "Edit Categories"), 
                GUILayout.Width(25)))
            {
                ShowCategoryEditor();
            }
            EditorGUILayout.EndHorizontal();

            _normalOffset = EditorGUILayout.Slider("Normal Offset", _normalOffset, 0, 0.5f);
            
            SaveSettings();
        }
        
        private void SaveSettings()
        {
            EditorPrefs.SetInt(LastCategoryKey, _selectedCategoryIndex);
            EditorPrefs.SetFloat(NormalOffsetKey, _normalOffset);
        }
        
        private void DrawControls()
        {
            EditorGUILayout.Space(5);

            var addContent = new GUIContent(" Add Prefab", EditorGUIUtility.IconContent("Toolbar Plus").image);
            if (GUILayout.Button(addContent, GUILayout.Height(25)))
            {
                SelectPrefab();
            }
        }
        
        private void DrawPrefabGrid()
        {
            EditorGUILayout.Space(5);
            
            var filteredPrefabs = GetFilteredPrefabs();
            
            if (filteredPrefabs.Count == 0)
            {
                EditorGUILayout.HelpBox("No prefabs found. Add some!", MessageType.Warning);
                return;
            }
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            
            int itemsPerRow = Mathf.Max(1, (int)(position.width - 40) / 100);
            int index = 0;
            
            foreach (var prefabData in filteredPrefabs)
            {
                if (index % itemsPerRow == 0)
                {
                    if (index > 0) EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
                
                DrawPrefabTile(prefabData);
                index++;
            }
            
            if (filteredPrefabs.Count > 0) EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }
        
        private List<AssetManagerDatabase.PrefabData> GetFilteredPrefabs()
        {
            var filtered = _database.prefabs.Where(p => p.prefab != null).ToList();

            if (_selectedCategoryIndex > 0)
            {
                var category = _database.categories[_selectedCategoryIndex - 1];
                filtered = filtered.Where(p => category.prefabGUIDs.Contains(p.guid)).ToList();
            }

            if (!string.IsNullOrEmpty(_searchString))
            {
                filtered = filtered.Where(p => 
                    (p.customName ?? p.prefab.name).ToLower().Contains(_searchString.ToLower())).ToList();
            }
            
            return filtered;
        }
        
        private void DrawPrefabTile(AssetManagerDatabase.PrefabData prefabData)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(110), GUILayout.Height(110));
            
            GameObject prefab = prefabData.prefab;
            string displayName = string.IsNullOrEmpty(prefabData.customName) ? prefab.name : prefabData.customName;

            Rect tileRect = GUILayoutUtility.GetRect(110, 110);

            EditorGUI.DrawRect(new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 2, tileRect.width - 2), 
                new Color(0.2f, 0.2f, 0.2f, 0.3f));

            EditorGUI.DrawRect(new Rect(tileRect.x, tileRect.y, tileRect.width, 1), Color.grey);
            EditorGUI.DrawRect(new Rect(tileRect.x, tileRect.y, 1, tileRect.height), Color.grey);
            EditorGUI.DrawRect(new Rect(tileRect.x + tileRect.width - 1, tileRect.y, 1, tileRect.height), Color.grey);
            EditorGUI.DrawRect(new Rect(tileRect.x, tileRect.y + tileRect.height - 1, tileRect.width, 1), Color.grey);

            Texture2D preview = AssetPreview.GetAssetPreview(prefab);
            if (preview != null)
            {
                var previewRect = new Rect(tileRect.x + 5, tileRect.y + 5, tileRect.width - 10, tileRect.width - 20);
                GUI.DrawTexture(previewRect, preview, ScaleMode.ScaleToFit);
            }

            if (Event.current.type == EventType.MouseDown && tileRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 0)
                {
                    StartPlacingPrefab(prefab);
                }
                else if (Event.current.button == 1)
                {
                    ShowPrefabContextMenu(prefabData);
                }
                Event.current.Use();
            }

            Rect nameRect = new Rect(tileRect.x, tileRect.yMax - 20, tileRect.width, 20);
            EditorGUI.DrawRect(nameRect, new Color(0, 0, 0, 0.7f));
                
            GUIStyle nameStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
                normal = { textColor = Color.white }
            };
                
            EditorGUI.LabelField(nameRect, displayName, nameStyle);
            
            EditorGUILayout.EndVertical();
        }
        
        private void ShowPrefabContextMenu(AssetManagerDatabase.PrefabData prefabData)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Rename..."), false, () => RenamePrefab(prefabData));
            menu.AddSeparator("");

            foreach (var category in _database.categories)
            {
                menu.AddItem(new GUIContent($"Move To Category/{category.name}"), false, 
                    () => MovePrefabToCategory(prefabData, category.name));
            }
            
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Remove From Category"), false, 
                () => RemovePrefabFromCurrentCategory(prefabData));
            menu.AddItem(new GUIContent("Delete From Database"), false, 
                () => DeletePrefab(prefabData));
            
            menu.ShowAsContext();
        }
        
        private void ShowSettingsMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Edit Categories..."), false, ShowCategoryEditor);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Delete All Prefabs..."), false, () =>
            {
                if (EditorUtility.DisplayDialog("Delete All", 
                    "Are you sure you want to delete ALL prefabs from the database?", "Delete", "Cancel"))
                {
                    _database.prefabs.Clear();
                    foreach (var category in _database.categories)
                    {
                        category.prefabGUIDs.Clear();
                    }
                    SaveDatabase();
                }
            });
            menu.ShowAsContext();
        }
        
        private void ShowCategoryEditor()
        {
            var window = EditorWindow.GetWindow<CategoryEditor>("Edit Categories");
            window.Initialize(_database, SaveDatabase);
        }
        
        private void RenamePrefab(AssetManagerDatabase.PrefabData prefabData)
        {
            var newName = EditorInputDialog.Show("Rename Prefab", "Enter new name:", 
                prefabData.customName ?? prefabData.prefab.name);
            
            if (!string.IsNullOrEmpty(newName))
            {
                prefabData.customName = newName;
                SaveDatabase();
            }
        }
        
        private void MovePrefabToCategory(AssetManagerDatabase.PrefabData prefabData, string categoryName)
        {
            foreach (var category in _database.categories)
            {
                category.prefabGUIDs.Remove(prefabData.guid);
            }

            var targetCategory = _database.categories.FirstOrDefault(c => c.name == categoryName);
            if (targetCategory != null && !targetCategory.prefabGUIDs.Contains(prefabData.guid))
            {
                targetCategory.prefabGUIDs.Add(prefabData.guid);
            }
            
            SaveDatabase();
        }
        
        private void RemovePrefabFromCurrentCategory(AssetManagerDatabase.PrefabData prefabData)
        {
            if (_selectedCategoryIndex > 0)
            {
                var category = _database.categories[_selectedCategoryIndex - 1];
                category.prefabGUIDs.Remove(prefabData.guid);
                SaveDatabase();
            }
        }
        
        private void DeletePrefab(AssetManagerDatabase.PrefabData prefabData)
        {
            _database.RemovePrefab(prefabData.guid);
            SaveDatabase();
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

                if (prefab != null)
                {
                    string guid = AssetDatabase.AssetPathToGUID(path);
                    string category = _database.categories[_selectedCategoryIndex - 1].name;
                    _database.AddPrefabToCategory(guid, category);
                    SaveDatabase();
                }
            }
        }
        
        private void SaveDatabase()
        {
            EditorUtility.SetDirty(_database);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        private void HandleDragAndDrop()
        {
            var fullWindowRect = new Rect(0, 0, position.width, position.height);
            
            if (!fullWindowRect.Contains(Event.current.mousePosition)) return;
            
            if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                
                if (Event.current.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    
                    if (_selectedCategoryIndex == 0)
                    {
                        EditorUtility.DisplayDialog("Asset Manager",
                            "You can't add prefab in the 'All' category. Please select something different.", "OK");
                        return;
                    }
                    
                    bool anyAdded = false;
                    foreach (var obj in DragAndDrop.objectReferences)
                    {
                        if (obj is GameObject go && PrefabUtility.GetPrefabAssetType(go) != PrefabAssetType.NotAPrefab)
                        {
                            string path = AssetDatabase.GetAssetPath(go);
                            string guid = AssetDatabase.AssetPathToGUID(path);
                            string category = _database.categories[_selectedCategoryIndex - 1].name;
                            
                            if (!_database.prefabs.Exists(p => p.guid == guid))
                            {
                                _database.prefabs.Add(new AssetManagerDatabase.PrefabData 
                                { 
                                    guid = guid, 
                                    prefab = go 
                                });
                            }
                            
                            _database.AddPrefabToCategory(guid, category);
                            anyAdded = true;
                        }
                    }
                    
                    if (anyAdded)
                    {
                        SaveDatabase();
                    }
                }
                
                Event.current.Use();
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
            if (!_isPlacingPrefab || _currentPrefabInstance == null) return;
            
            Event e = Event.current;
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            Collider[] colliders = _currentPrefabInstance.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
            
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Default")))
            {
                _currentPrefabInstance.transform.position = hit.point + hit.normal * _normalOffset;
            }

            foreach (var collider in colliders)
            {
                collider.enabled = true;
            }

            Handles.color = Color.green;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Default")))
            {
                Handles.SphereHandleCap(0, hit.point, Quaternion.identity, 0.25f, EventType.Repaint);
            }

            if (e.type == EventType.MouseDown && e.button == 0 && !e.control && !e.alt)
            {
                PlacePrefab();
                e.Use();
            }
            
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                DestroyImmediate(_currentPrefabInstance);
                _currentPrefabInstance = null;
                _isPlacingPrefab = false;
                SceneView.duringSceneGui -= DuringSceneGUI;
                e.Use();
            }
            
            SceneView.RepaintAll();
        }
    }

    public class CategoryEditor : EditorWindow
    {
        private AssetManagerDatabase _database;
        private Action _saveCallback;
        private Vector2 _scrollPos;
        private string _newCategoryName = "";
        private bool[] _categoryFoldouts;
        
        public void Initialize(AssetManagerDatabase database, Action saveCallback)
        {
            _database = database;
            _saveCallback = saveCallback;
            _categoryFoldouts = new bool[_database.categories.Count];
        }
        
        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Category Management", 
                new GUIStyle(EditorStyles.boldLabel) { fontSize = 18, alignment = TextAnchor.MiddleCenter });
            
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            _newCategoryName = EditorGUILayout.TextField("New Category:", _newCategoryName);
            if (GUILayout.Button("Add", GUILayout.Width(50)) && !string.IsNullOrWhiteSpace(_newCategoryName))
            {
                if (_database.categories.All(c => c.name != _newCategoryName))
                {
                    _database.categories.Add(new AssetManagerDatabase.Category { name = _newCategoryName.Trim() });
                    _saveCallback?.Invoke();
                    _newCategoryName = "";
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(10);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            
            for (int i = 0; i < _database.categories.Count; i++)
            {
                var category = _database.categories[i];
                
                EditorGUILayout.BeginVertical(new GUIStyle("box"));
                
                EditorGUILayout.BeginHorizontal();
                _categoryFoldouts[i] = EditorGUILayout.Foldout(_categoryFoldouts[i], category.name, true);

                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("editicon.sml").image, "Rename"), 
                        GUILayout.Width(25)))
                {
                    RenameCategory(i);
                }

                if (GUILayout.Button(new GUIContent(EditorGUIUtility.IconContent("Toolbar Minus").image, "Delete"), 
                        GUILayout.Width(25)))
                {
                    if (EditorUtility.DisplayDialog("Delete Category", 
                        $"Delete '{category.name}'?", "Delete", "Cancel"))
                    {
                        _database.categories.RemoveAt(i);
                        _saveCallback?.Invoke();
                        return;
                    }
                }
                
                EditorGUILayout.EndHorizontal();
                
                if (_categoryFoldouts[i])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField($"Prefabs: {category.prefabGUIDs.Count}");

                    for (int j = 0; j < Mathf.Min(3, category.prefabGUIDs.Count); j++)
                    {
                        var guid = category.prefabGUIDs[j];
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        if (prefab != null)
                        {
                            EditorGUILayout.LabelField($"• {prefab.name}");
                        }
                    }
                    
                    if (category.prefabGUIDs.Count > 3)
                    {
                        EditorGUILayout.LabelField($"... and {category.prefabGUIDs.Count - 3} more");
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        private void RenameCategory(int index)
        {
            var category = _database.categories[index];
            var newName = EditorInputDialog.Show("Rename Category", "New name:", category.name);
            
            if (!string.IsNullOrEmpty(newName) && newName != category.name)
            {
                category.name = newName;
                _saveCallback?.Invoke();
            }
        }
    }

    public static class EditorInputDialog
    {
        public static string Show(string title, string message, string defaultValue)
        {
            string result = null;
            
            var window = EditorWindow.GetWindow<InputDialogWindow>(true);
            window.titleContent = new GUIContent(title);
            window.Initialize(message, defaultValue, val => result = val);
            window.ShowModalUtility();
            
            return result;
        }
        
        private class InputDialogWindow : EditorWindow
        {
            private string _message;
            private string _value;
            private Action<string> _callback;
            
            public void Initialize(string message, string defaultValue, Action<string> callback)
            {
                _message = message;
                _value = defaultValue;
                _callback = callback;
                minSize = new Vector2(300, 100);
            }
            
            private void OnGUI()
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(_message);
                _value = EditorGUILayout.TextField(_value);
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("OK"))
                {
                    _callback?.Invoke(_value);
                    Close();
                }
                if (GUILayout.Button("Cancel"))
                {
                    Close();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}