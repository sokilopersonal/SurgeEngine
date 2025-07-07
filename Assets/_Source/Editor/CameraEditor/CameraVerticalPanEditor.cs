using SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects;
using UnityEditor;

namespace SurgeEngine._Source.Editor.CameraEditor
{
    [CustomEditor(typeof(ObjCameraParallel))]
    public class CameraVerticalPanEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Pans the player's camera:\n- Camera moves with the player at a set distance & offset\n- Looks in the object's forward direction", MessageType.Info);
        
            base.OnInspectorGUI();
        }
    }
}
