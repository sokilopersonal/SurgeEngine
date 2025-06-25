using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace SurgeEngine._Source.Editor.ObjectSet
{
    [InitializeOnLoad]
    static class ObjectSetToolbar
    {
        static ObjectSetToolbar()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
        }

        private static void OnLeftToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            var style = new GUIStyle(EditorStyles.toolbarButton);
            style.fontSize = 13;
            style.fontStyle = FontStyle.Bold;
            if (GUILayout.Button("Asset Manager", style))
            {
                ObjectSet.ShowWindow();
            }
        }
    }
}