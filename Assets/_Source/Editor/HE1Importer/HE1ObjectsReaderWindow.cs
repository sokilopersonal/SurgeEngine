using System;
using UnityEditor;
using UnityEngine;

namespace SurgeEngine._Source.Editor.HE1Importer
{
    public class HE1ObjectsReaderWindow : EditorWindow
    {
        [MenuItem("SurgeEngine/Hedgehog Engine 1 Tools/Objects Reader (Importer)")]
        private static void ShowWindow()
        {
            var window = GetWindow<HE1ObjectsReaderWindow>();
            window.titleContent = new GUIContent("HE1 Objects Reader");
            window.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Load XML", GUILayout.Height(30)))
            {
                var xml = EditorUtility.OpenFilePanel("Load XML", "", "xml");
                if (!string.IsNullOrEmpty(xml))
                {
                    HE1ObjectsImporter.ReadObjects(xml);
                }
            }
        }
    }
}