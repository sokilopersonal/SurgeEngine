using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.CommonObjects.System;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Editor
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
                if (!Object.FindFirstObjectByType<Stage>())
                {
                    var stage = new GameObject("+ Stage").AddComponent<Stage>();
                    Undo.RegisterCreatedObjectUndo(stage, "Add Stage");
                }

                if (!Object.FindFirstObjectByType<SceneContext>())
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<SceneContext>("Assets/Prefabs/DI/SceneContext.prefab");
                    var sceneContext = PrefabUtility.InstantiatePrefab(prefab, GameObject.Find("+ Stage").transform);
                    Undo.RegisterCreatedObjectUndo(sceneContext, "Add SceneContext");
                }

                if (!GameObject.Find("- Volumes"))
                {
                    var volumes = new GameObject("- Volumes");
                    Undo.RegisterCreatedObjectUndo(volumes, "Add Volumes folder");
                }

                if (!GameObject.Find("- SetData"))
                {
                    var setdata = new GameObject("- SetData")
                    {
                        tag = "SetData"
                    };
                    Undo.RegisterCreatedObjectUndo(setdata, "Add SetData folder");
                }

                if (!GameObject.Find("- Geometry"))
                {
                    var geometry = new GameObject("- Geometry");
                    Undo.RegisterCreatedObjectUndo(geometry, "Add Geometry folder");
                }

                if (!GameObject.Find("- Lighting"))
                {
                    var lighting = new GameObject("- Lighting");
                    Undo.RegisterCreatedObjectUndo(lighting, "Add Lighting folder");
                }
            }
        }
    }
}