using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Editor
{
    public static class SetStartAndExitPath
    {
        [MenuItem("Surge Engine/Set Collision Path", false, 1)]
        public static void Set()
        {
            var selection = Selection.activeGameObject;
            if (selection == null) return;
            
            var container = selection.transform.GetComponentInChildren<SplineContainer>();
            var spline = container.Spline;

            var start = selection.transform.GetChild(0);
            var end = selection.transform.GetChild(1);

            spline.Evaluate(0f, out var startPos, out var startTangent, out _);
            spline.Evaluate(1f, out var endPos, out var endTangent, out _);
            
            start.position = container.transform.TransformPoint(startPos);
            start.forward = container.transform.TransformDirection(startTangent);
            end.position = container.transform.TransformPoint(endPos);
            end.forward = container.transform.TransformDirection(endTangent);
            
            Undo.RegisterCompleteObjectUndo(selection, "Set Start Path");
            EditorUtility.SetDirty(selection);
        }
    }
}