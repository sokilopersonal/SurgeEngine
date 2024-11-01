using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;

namespace SurgeEngine.Editor
{
    public class AssetManager : EditorWindow
    {
        [Serializable]
        public class PrefabData
        {
            public string path;
            public string category;
            public GameObject prefab;

            public PrefabData(string path, string category, GameObject prefab)
            {
                this.path = path;
                this.category = category;
                this.prefab = prefab;
            }
        }

        private List<PrefabData> prefabDataList = new List<PrefabData>();
        private Vector2 scrollPosition;
        private const string SaveFilePath = "Assets/Editor/SelectedPrefabs.json";
        private string[] categories;
        private int selectedCategoryIndex = 0;
        private GameObject currentPrefabInstance;
        private bool isPlacingPrefab;
        
        [MenuItem("Surge Engine/Asset Manager")]
        private static void ShowWindow()
        {
            var window = GetWindow<AssetManager>();
            window.titleContent = new GUIContent("Asset Manager");
            window.Show();
        }

        private void OnEnable()
        {
            LoadAssetsList();
        }

        private void OnGUI()
        {
            GUILayout.Label("Asset Manager", EditorStyles.boldLabel);
            categories = new[] { "All", "Common", "Enemies", "Ring Groups", "Cameras" };
            
            GUILayout.Label("Category:");
            selectedCategoryIndex = EditorGUILayout.Popup(selectedCategoryIndex, categories);

            if (GUILayout.Button("Add Prefab"))
            {
                SelectPrefab();
            }

            GUILayout.Space(10);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            int itemsPerRow = Mathf.Max(1, (int)(position.width / 115));
            int drawnCount = 0;
            EditorGUILayout.BeginHorizontal();
            foreach (var prefabData in prefabDataList)
            {
                if (selectedCategoryIndex != 0 && prefabData.category != categories[selectedCategoryIndex])
                    continue;

                if (DrawPrefabTile(prefabData))
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
            GUILayout.EndScrollView();

            if (GUILayout.Button("Save Assets List"))
            {
                SaveAssetsList();
            }
        }

        private bool DrawPrefabTile(PrefabData prefabData)
        {
            bool rightClick = false;
            EditorGUILayout.BeginVertical(GUILayout.Width(100));
            {
                Texture2D previewTexture = AssetPreview.GetAssetPreview(prefabData.prefab);

                int size = 110;
                if (GUILayout.Button(previewTexture, GUILayout.Width(size), GUILayout.Height(size)))
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
                    normal = { textColor = Color.white },
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 13
                };
                GUILayout.Label(prefabData.prefab.name, style, GUILayout.Width(100));
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
            int index = prefabDataList.IndexOf(prefabData);
            int newIndex = Mathf.Clamp(index + direction, 0, prefabDataList.Count - 1);
            if (index != newIndex)
            {
                prefabDataList.RemoveAt(index);
                prefabDataList.Insert(newIndex, prefabData);
            }
        }

        private void MovePrefabToTop(PrefabData prefabData)
        {
            prefabDataList.Remove(prefabData);
            prefabDataList.Insert(0, prefabData);
        }

        private void MovePrefabToBottom(PrefabData prefabData)
        {
            prefabDataList.Remove(prefabData);
            prefabDataList.Add(prefabData);
        }

        private void SelectPrefab()
        {
            if (selectedCategoryIndex == 0)
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

                if (prefab != null && !prefabDataList.Exists(data => data.prefab == prefab))
                {
                    string category = categories[selectedCategoryIndex];
                    prefabDataList.Add(new PrefabData(path, category, prefab));
                }
            }
        }

        private void RemovePrefab(PrefabData prefabData)
        {
            prefabDataList.Remove(prefabData);
        }

        private void SaveAssetsList()
        {
            var jsonList = new List<PrefabData>();
            foreach (var data in prefabDataList)
            {
                jsonList.Add(new PrefabData(data.path, data.category, null));
            }

            try
            {
                string json = JsonConvert.SerializeObject(jsonList, Formatting.Indented);
                File.WriteAllText(SaveFilePath, json);
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError($"[Asset Manager] There's an exception: {e}");
            }
        }

        private void LoadAssetsList()
        {
            prefabDataList.Clear();

            if (File.Exists(SaveFilePath))
            {
                string json = File.ReadAllText(SaveFilePath);
                var jsonList = JsonConvert.DeserializeObject<List<PrefabData>>(json);

                foreach (var data in jsonList)
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(data.path);
                    if (prefab != null)
                    {
                        prefabDataList.Add(new PrefabData(data.path, data.category, prefab));
                    }
                }
            }
        }

        private void StartPlacingPrefab(GameObject prefab)
        {
            if (currentPrefabInstance != null)
            {
                DestroyImmediate(currentPrefabInstance);
            }

            currentPrefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            isPlacingPrefab = true;
            SceneView.duringSceneGui += DuringSceneGUI;
            FocusWindowIfItsOpen<SceneView>();
        }
        
        private void PlacePrefab()
        {
            if (currentPrefabInstance != null)
            {
                Undo.RegisterCreatedObjectUndo(currentPrefabInstance, "Place Prefab");
                Selection.activeObject = currentPrefabInstance;
                currentPrefabInstance = null;
                isPlacingPrefab = false;
                SceneView.duringSceneGui -= DuringSceneGUI;
            }
        }
        
        private void DuringSceneGUI(SceneView sceneView)
        {
            if (isPlacingPrefab && currentPrefabInstance != null)
            {
                Event e = Event.current;
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Default")))
                {
                    currentPrefabInstance.transform.position = hit.point;
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
                    DestroyImmediate(currentPrefabInstance);
                    e.Use();
                }

                SceneView.RepaintAll();
            }
        }
    }
}
