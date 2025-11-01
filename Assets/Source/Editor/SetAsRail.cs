using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility.Rails;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Source.Editor
{
    public static class SetAsRail
    {
        [MenuItem("GameObject/Surge Engine/Set As Rail")]
        public static void SetAsRailObject()
        {
            var selection = Selection.activeGameObject;

            if (!selection.TryGetComponent(out SplineContainer container))
            {
                Debug.Log("Object is not a spline. Please add a spline container.");
                return;
            }

            if (selection.TryGetComponent(out Rail rail))
            {
                Debug.Log("Object already has rail component.");
                return;
            }
            
            selection.AddComponent<Rail>();
            
            Undo.RegisterCompleteObjectUndo(selection, "Set As Rail");
        }
    }
}