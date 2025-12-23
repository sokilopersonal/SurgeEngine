using UnityEditor;
using UnityEngine;

namespace SurgeEngine.Source.Editor.HE1Importer
{
    public class HE1Tools : UnityEditor.Editor
    {
        [MenuItem("Surge Engine/Hedgehog Engine 1 Tools/Import XML", priority = 0)]
        public static void ImportXML()
        {
            var xml = EditorUtility.OpenFilePanel("Load XML", "", "xml");
            if (!string.IsNullOrEmpty(xml))
            {
                HE1ObjectsImporter.ReadObjects(xml);
            }
        }

        [MenuItem("Surge Engine/Hedgehog Engine 1 Tools/Import Splines XML", priority = 1)]
        public static void ImportSplinesXML()
        {
            var xml = EditorUtility.OpenFilePanel("Load XML", "", "xml");
            if (!string.IsNullOrEmpty(xml))
            {
                HE1SplinesImporter.ReadSpline(xml);
            }
        }
    }
}