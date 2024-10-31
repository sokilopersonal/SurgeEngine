using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace SurgeEngine.Editor
{
    public class AssetManager : EditorWindow
    {
        private List<GameObject> selectedPrefabs = new List<GameObject>();
        private Vector2 scrollPosition;
        private const string SaveFilePath = "Assets/Editor/SelectedPrefabs.txt";
        
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

            if (GUILayout.Button("Add Prefab"))
            {
                SelectPrefab();
            }

            GUILayout.Space(10);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < selectedPrefabs.Count; i++)
            {
                if (selectedPrefabs[i] != null)
                {
                    if (DrawPrefabTile(selectedPrefabs[i]))
                    {
                        ShowContextMenu(i);
                    }
                }
                
                if ((i + 1) % 3 == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUILayout.Space(5);
    
            if (GUILayout.Button("Save Assets List"))
            {
                SaveAssetsList();
            }

            if (GUILayout.Button("Load Assets List"))
            {
                LoadAssetsList();
            }
        }

        private bool DrawPrefabTile(GameObject prefab)
        {
            bool rightClick = false;
            
            EditorGUILayout.BeginVertical(GUILayout.Width(120));
            {
                Texture2D previewTexture = AssetPreview.GetAssetPreview(prefab);
        
                if (GUILayout.Button(previewTexture, GUILayout.Width(100), GUILayout.Height(100)))
                {
                    if (Event.current.button == 0)
                    {
                        StartPlacingPrefab(prefab);
                    }
                    else if (Event.current.button == 1)
                    {
                        rightClick = true;
                    }
                }

                GUIStyle style = new GUIStyle();
                style.font = EditorStyles.boldFont;
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 12;
                GUILayout.Label(prefab.name, style, GUILayout.Width(100));
            }
            EditorGUILayout.EndVertical();

            return rightClick;
        }

        private void ShowContextMenu(int index)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Remove"), false, () => RemovePrefab(index));
            menu.ShowAsContext();
        }

        private void SelectPrefab()
        {
            string path = EditorUtility.OpenFilePanelWithFilters("Select Prefab", "Assets/Prefabs", new[] { "Prefab", "prefab" });
            if (!string.IsNullOrEmpty(path))
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null && !selectedPrefabs.Contains(prefab))
                {
                    selectedPrefabs.Add(prefab);
                }
            }
        }

        private void RemovePrefab(int index)
        {
            selectedPrefabs.RemoveAt(index);
        }

        private void SaveAssetsList()
        {
            List<string> assetPaths = new List<string>();
            foreach (GameObject prefab in selectedPrefabs)
            {
                assetPaths.Add(AssetDatabase.GetAssetPath(prefab));
                Debug.Log(AssetDatabase.GetAssetPath(prefab));
            }

            File.WriteAllLines(SaveFilePath, assetPaths);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Save Assets List", "Asset list saved successfully!", "OK");
        }

        private void LoadAssetsList()
        {
            selectedPrefabs.Clear();

            if (File.Exists(SaveFilePath))
            {
                string[] assetPaths = File.ReadAllLines(SaveFilePath);

                foreach (string path in assetPaths)
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab != null)
                    {
                        selectedPrefabs.Add(prefab);
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
                
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Default"), QueryTriggerInteraction.Ignore))
                {
                    currentPrefabInstance.transform.position = hit.point;
                }

                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    PlacePrefab();
                    e.Use();
                }
                
                // TODO: Fix this action doesn't work until I press RMB (?)
                if (e.type == EventType.KeyUp && e.keyCode == KeyCode.Escape)
                {
                    DestroyImmediate(currentPrefabInstance);
                    e.Use();
                }

                SceneView.RepaintAll();
            }
        }
    }
}
