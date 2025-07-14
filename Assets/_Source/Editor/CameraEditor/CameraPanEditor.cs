using SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects;
using UnityEditor;

namespace SurgeEngine._Source.Editor.CameraEditor
{
    [CustomEditor(typeof(ObjCameraPan))]
    public class CameraPanEditor : CameraBaseEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Pans the player's camera:\n- Stationary camera\n- Looks at the player", MessageType.Info);
        
            base.OnInspectorGUI();
        }
    }
}
