using SurgeEngine.Code.Core.Actor.System;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine._Source.Editor
{
    public static class PlaceActor
    {
        [MenuItem("Surge Engine/Place Actor")]
        public static void Place()
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            Camera camera = sceneView.camera;
            
            Ray ray = new Ray(camera.transform.position, camera.transform.forward);

            var instance = Object.FindFirstObjectByType<ActorBase>();
            if (instance != null)
            {
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    instance.transform.parent.position = hit.point;
                    instance.transform.parent.forward = Vector3.Cross(camera.transform.right, Vector3.up);
                    
                    Undo.RegisterCompleteObjectUndo(instance, "Move Actor");
                }
            }
            else
            {
                if (Physics.Raycast(ray, out RaycastHit hit2))
                {
                    var actor = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Sonic/Actor.prefab");
                    var actorInstance = PrefabUtility.InstantiatePrefab(actor) as GameObject;
                    actorInstance.transform.position = hit2.point;
                    actorInstance.transform.forward = Vector3.Cross(camera.transform.right, Vector3.up);
                    Undo.RegisterCreatedObjectUndo(actorInstance, "Create Actor Instance");
                }
            }
        }
    }
}