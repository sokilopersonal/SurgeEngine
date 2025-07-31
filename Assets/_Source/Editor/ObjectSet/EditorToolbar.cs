using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace SurgeEngine._Source.Editor.ObjectSet
{
    [InitializeOnLoad]
    static class EditorToolbar
    {
        static EditorToolbar()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
            ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
        }

        private static void OnLeftToolbarGUI()
        {
            GUILayout.FlexibleSpace();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target FPS:", GUILayout.Width(70));
            int fps = EditorGUILayout.IntSlider("", Application.targetFrameRate, 30, 120, GUILayout.Width(200));
            if (Application.isPlaying)
            {
                if (fps != Application.targetFrameRate)
                {
                    Application.targetFrameRate = fps;
                }
            }
            GUILayout.EndHorizontal();
            
            EditorGUILayout.Space(15);
            
            var style = new GUIStyle(EditorStyles.toolbarButton);
            style.fontSize = 13;
            style.fontStyle = FontStyle.Bold;
            if (GUILayout.Button("Asset Manager", style))
            {
                ObjectSet.ShowWindow();
            }
        }

        private static void OnRightToolbarGUI()
        {
            GUILayout.FlexibleSpace();

            
            
        }
    }
}