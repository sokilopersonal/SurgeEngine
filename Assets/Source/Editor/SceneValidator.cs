using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine.Source.Editor
{
    public static class SceneValidator
    {
        [MenuItem("Surge Engine/Validate Scene")]
        public static void ValidateScene()
        {
            string msg = "It's better to validate a scene for better navigation. This action will add the necessary folders to the scene." +
                         " \nAre you sure you want to continue?";
            if (EditorUtility.DisplayDialog("Scene Validation", msg, "Yes", "No"))
            {
                var currentScene = SceneManager.GetActiveScene();
                var additiveScene = EditorSceneManager.OpenScene("Assets/Source/Scenes/Template/GameplayScene.unity", OpenSceneMode.Additive);

                foreach (var rootGameObject in currentScene.GetRootGameObjects())
                    Undo.RegisterFullObjectHierarchyUndo(rootGameObject, "Validate Scene");

                var currentRoots = currentScene.GetRootGameObjects();

                foreach (var rootObj in additiveScene.GetRootGameObjects())
                {
                    bool exists = currentRoots.Any(o => o.name == rootObj.name);
                    if (!exists)
                    {
                        SceneManager.MoveGameObjectToScene(rootObj, currentScene);
                    }
                    else
                    {
                        Object.DestroyImmediate(rootObj);
                    }
                }

                EditorSceneManager.CloseScene(additiveScene, true);
                EditorSceneManager.MarkSceneDirty(currentScene);
                Debug.Log("Scene validated successfully.");
            }
        }

        private static bool CheckSceneSetup()
        {
            if (Object.FindFirstObjectByType<SceneContext>())
            {
                return true;
            }
            
            if (GameObject.Find("- Volumes"))
            {
                return true;
            }
            
            if (GameObject.Find("- SetData"))
            {
                return true;
            }
            
            if (GameObject.Find("- Geometry"))
            {
                return true;
            }
            
            if (GameObject.Find("- Lighting"))
            {
                return true;
            }

            return false;
        }
    }
}