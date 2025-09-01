using SurgeEngine._Source.Code.Gameplay.CommonObjects.CameraObjects;
using UnityEditor;

namespace SurgeEngine._Source.Editor.CameraEditor
{
    [CustomEditor(typeof(ObjCameraFix))]
    public class CameraFixEditor : CameraBaseEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Translates & Rotates the player's camera:\n- Fixed position & rotation", MessageType.Info);
        
            base.OnInspectorGUI();
        }
    }
}
