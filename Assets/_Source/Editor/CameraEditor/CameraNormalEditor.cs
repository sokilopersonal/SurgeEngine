using SurgeEngine._Source.Code.Gameplay.CommonObjects.CameraObjects;
using UnityEditor;

namespace SurgeEngine._Source.Editor.CameraEditor
{
    [CustomEditor(typeof(ObjCameraNormal))]
    public class CameraNormalEditor : CameraBaseEditor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Pans the player's camera:\n- Camera changes its distance, vertical offset and base field of view", MessageType.Info);
        
            base.OnInspectorGUI();
        }
    }
}