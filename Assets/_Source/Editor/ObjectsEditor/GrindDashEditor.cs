using SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility.Rails;
using UnityEditor;
using UnityEngine.UIElements;

namespace SurgeEngine._Source.Editor.ObjectsEditor
{
    [CustomEditor(typeof(GrindDash))]
    public class GrindDashEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();
            SerializedProperty iterator = serializedObject.GetIterator();
            SerializedProperty container = serializedObject.FindProperty("container");

            for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                if ((iterator.propertyPath == "splineTime" 
                     || iterator.propertyPath == "verticalOffset") && container.objectReferenceValue == null)
                    continue;

                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}