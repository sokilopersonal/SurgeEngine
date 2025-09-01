using SurgeEngine._Source.Code.Gameplay.CommonObjects.CameraObjects;
using UnityEditor;

namespace SurgeEngine._Source.Editor.CameraEditor
{
    [CustomEditor(typeof(ObjCameraPoint))]
    public class CameraPointEditor : CameraBaseEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Pans the player's camera:\n-" +
                                    " Camera moves with the player at a set distance & offset\n-" +
                                    " Looks in the object's forward direction + offset", MessageType.Info);
            
            base.OnInspectorGUI();
        }
    }
}