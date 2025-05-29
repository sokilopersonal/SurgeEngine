using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine._Source.Editor
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
                if (!CheckSceneSetup())
                {
                    var currentScene = SceneManager.GetActiveScene();
                    var additiveScene = EditorSceneManager.OpenScene("Assets/_Source/Scenes/Template/GameplayScene.unity", OpenSceneMode.Additive);
                    
                    foreach (var rootGameObject in currentScene.GetRootGameObjects()) Undo.RegisterFullObjectHierarchyUndo(rootGameObject, "Validate Scene");

                    foreach (var rootObj in additiveScene.GetRootGameObjects())
                    {
                        SceneManager.MoveGameObjectToScene(rootObj, currentScene);
                    }
                    
                    EditorSceneManager.CloseScene(additiveScene, true);
                }
                else
                {
                    EditorUtility.DisplayDialog("Scene Validation", "Scene is already validated. " +
                                                                    "If you want to revalidate it: " +
                                                                    "remove GameplaySceneContext, Volumes, SetData, Geometry and Lighting objects", "Ok");
                }
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