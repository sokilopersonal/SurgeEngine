using UnityEditor;
using UnityEngine;

namespace SurgeEngine.Source.Editor
{
    public static class HomingTargetCreator
    {
        [MenuItem("GameObject/Surge Engine/Homing Target", false, 0)]
        public static void Create()
        {
            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/CommonObjects/HomingTarget.prefab");
            
            Transform selection = Selection.activeTransform;
            
            if (selection != null)
            {
                var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                instance.transform.parent = selection;
                instance.transform.localPosition = Vector3.up * 0.25f;
                instance.transform.localRotation = Quaternion.identity;
                Undo.RegisterCreatedObjectUndo(instance, "Create Homing Target");
            }
        }
    }
}