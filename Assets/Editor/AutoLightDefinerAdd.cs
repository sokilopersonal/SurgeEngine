using SurgeEngine.Code.Gameplay.CommonObjects.Lighting;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine.Editor
{
    [InitializeOnLoad]
    public static class AutoLightDefinerAdd
    {
        static AutoLightDefinerAdd()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged()
        {
            var selected = Selection.activeGameObject;
            if (selected)
            {
                if (selected.GetComponent<Light>() && !selected.GetComponent<LightDefiner>())
                {
                    Undo.AddComponent<LightDefiner>(selected);
                    Debug.Log("[Framework] Auto added LightDefiner to the selected light: " + selected.name);
                }
            }
        }
    }
}