
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;

namespace SurgeEngine.Source.Code.Shaders
{
    public class GraphicsStateCollectionManager : MonoBehaviour
    {
        public enum Mode
        {
            Tracing,
            WarmUp
        };
        public Mode mode;

        public static GraphicsStateCollectionManager Instance;

        public GraphicsStateCollection[] collections;
        private const string CollectionFolderPath = "_Source/RenderStatesCollections/";

        private string _outputCollectionName;
        private GraphicsStateCollection _graphicsStateCollection;

        #if UNITY_EDITOR
        [ContextMenu("Update collection list")]
        public void UpdateCollectionList()
        {
            string[] collectionGUIDs = AssetDatabase.FindAssets("t:GraphicsStateCollection", new[] {"Assets/" + CollectionFolderPath});
            collections = new GraphicsStateCollection[collectionGUIDs.Length];
            for (int i = 0; i < collections.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(collectionGUIDs[i]);
                collections[i] = AssetDatabase.LoadAssetAtPath<GraphicsStateCollection>(path);
            }
            EditorUtility.SetDirty(this);
        }
        #endif

        private GraphicsStateCollection FindExistingCollection()
        {
            for (int i = 0; i < collections.Length; i++)
            {
                if (collections[i] != null)
                {
                    if (collections[i].runtimePlatform == Application.platform &&
                        collections[i].graphicsDeviceType == SystemInfo.graphicsDeviceType &&
                        collections[i].qualityLevelName == QualitySettings.names[QualitySettings.GetQualityLevel()])
                    {
                        return collections[i];
                    }
                }
            }

            return null;
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError("Only one instance of GraphicsStateCollectionManager is allowed!");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            if (mode == Mode.Tracing)
            {
                _graphicsStateCollection = FindExistingCollection();

                if (_graphicsStateCollection != null)
                {
                    _outputCollectionName = CollectionFolderPath + _graphicsStateCollection.name;
                }
                else
                {
                    int qualityLevelIndex = QualitySettings.GetQualityLevel();
                    string qualityLevelName = QualitySettings.names[qualityLevelIndex];
                    qualityLevelName = qualityLevelName.Replace(" ", "");

                    _outputCollectionName = string.Concat(CollectionFolderPath, "GfxState_", Application.platform, "_", SystemInfo.graphicsDeviceType.ToString(), "_", qualityLevelName);

                    _graphicsStateCollection = new GraphicsStateCollection();
                }

                Scene scene = SceneManager.GetActiveScene();
                Debug.Log("Tracing started for GraphicsStateCollection by Scene '" + scene.name + "'.");
                _graphicsStateCollection.BeginTrace();
            }
            else
            {
                GraphicsStateCollection collection = FindExistingCollection();

                if (collection != null)
                {
                    Scene scene = SceneManager.GetActiveScene();
                    Debug.Log("Scene '" + scene.name + "' started warming up " + collection.totalGraphicsStateCount + " GraphicsState entries.");
                    collection.WarmUp();
                }
            }
        }

        void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                if (mode == Mode.Tracing && _graphicsStateCollection != null)
                {
                    Debug.Log("Focus changed. Sending collection to Editor with " + _graphicsStateCollection.totalGraphicsStateCount + " GraphicsState entries.");
                    _graphicsStateCollection.SendToEditor(_outputCollectionName);
                }
            }
        }

        void OnDestroy()
        {
            if (mode == Mode.Tracing && _graphicsStateCollection != null)
            {
                _graphicsStateCollection.EndTrace();
                Debug.Log("Sending collection to Editor with " + _graphicsStateCollection.totalGraphicsStateCount + " GraphicsState entries.");
                _graphicsStateCollection.SendToEditor(_outputCollectionName);
            }
        }
        
#if UNITY_EDITOR
        [MenuItem("GameObject/Rendering/Graphics State Collection Manager")]
        private static void CreateGraphicsStateManager()
        {
            var go = new GameObject("GraphicsStateCollectionManager");
            var gst = go.AddComponent<GraphicsStateCollectionManager>();
            EditorGUIUtility.PingObject(go);
            Selection.activeGameObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create Graphics State Collection Manager");
            gst.UpdateCollectionList();
            gst.mode = Mode.WarmUp;
        }
#endif
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(GraphicsStateCollectionManager))]
    class GraphicsStateCollectionManagerEditor : Editor
    {
        private const string Message =
            "Right click on this component to fill the collection list automatically with the files from the GraphicsStateCollections folder. \n" +
            "Collection files with irrelevant platforms will be excluded from build automatically according to current build target platform by GraphicsStateCollectionStripper.";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.HelpBox(Message, MessageType.Info);
        }
    }
    #endif
}

