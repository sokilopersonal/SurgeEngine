using System.Linq;
using SurgeEngine.Code.Custom;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Editor
{
    public static class SetStartAndExitPath
    {
        [MenuItem("Surge Engine/Set Start Path", false, 1)]
        public static void SetStart()
        {
            var selection = Selection.activeGameObject;
            
            if (selection == null) return;
            
            var container = selection.transform.parent.GetComponentInChildren<SplineContainer>();
            var spline = container.Spline;

            var first = spline.Knots.ToArray()[0];
            selection.transform.position = container.transform.TransformPoint(first.Position + SurgeMath.Vector3ToFloat3(Vector3.up * 0.5f));
            selection.transform.rotation = first.Rotation;
            
            Undo.RegisterCompleteObjectUndo(selection, "Set Start Path");
            EditorUtility.SetDirty(selection);
        }
        
        [MenuItem("Surge Engine/Set End Path", false, 2)]
        public static void SetEnd()
        {
            var selection = Selection.activeGameObject;
            
            if (selection == null) return;
            
            var container = selection.transform.parent.GetComponentInChildren<SplineContainer>();
            var spline = container.Spline;

            var last = spline.Knots.ToArray().Last();
            selection.transform.position = container.transform.TransformPoint(last.Position + SurgeMath.Vector3ToFloat3(Vector3.up * 0.5f));
            selection.transform.rotation = last.Rotation;
            
            Undo.RegisterCompleteObjectUndo(selection, "Set End Path");
            EditorUtility.SetDirty(selection);
        }
    }
}