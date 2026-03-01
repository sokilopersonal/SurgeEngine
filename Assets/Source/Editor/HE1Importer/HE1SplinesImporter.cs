using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Source.Editor.HE1Importer
{
    public static class HE1SplinesImporter
    {
        private class SplineData
        {
            public string Name;
            public XElement Node;
            public XElement Geometry;
            public bool Selected;
        }

        private class SplineImporterWindow : EditorWindow
        {
            private List<SplineData> _splines;
            private Vector2 _scrollPosition;

            public static void ShowWindow(List<SplineData> splines)
            {
                var window = GetWindow<SplineImporterWindow>("Import Splines");
                window._splines = splines;
                window.Show();
            }

            private void OnGUI()
            {
                EditorGUILayout.LabelField("Select Splines to Import", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select All"))
                {
                    foreach (var spline in _splines)
                        spline.Selected = true;
                }
                if (GUILayout.Button("Deselect All"))
                {
                    foreach (var spline in _splines)
                        spline.Selected = false;
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                foreach (var spline in _splines)
                {
                    spline.Selected = EditorGUILayout.ToggleLeft(spline.Name, spline.Selected);
                }
                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space();

                if (GUILayout.Button("Import Selected"))
                {
                    ImportSelectedSplines();
                    Close();
                }
            }

            private void ImportSelectedSplines()
            {
                foreach (var splineData in _splines.Where(s => s.Selected))
                {
                    CreateSplineGameObject(splineData.Node, splineData.Geometry, splineData.Name);
                }
            }
        }
        private const string HESideViewTag = "@SV";
        private const string HEQuickstepTag = "@QS";
        private const string UnitySideViewTag = "SideView";
        private const string UnityQuickstepTag = "Quickstep";
        
        public static void ReadSpline(string xmlPath)
        {
            var doc = XDocument.Load(xmlPath);

            var geometries = doc.Descendants("geometry").ToDictionary(
                g => g.Attribute("id")?.Value,
                g => g
            );

            var splineDataList = new List<SplineData>();
            var nodes = doc.Descendants("node");

            foreach (var node in nodes)
            {
                var instanceUrl = node.Element("instance")?.Attribute("url")?.Value;
                if (string.IsNullOrEmpty(instanceUrl)) continue;

                var geomId = instanceUrl.TrimStart('#');
                if (!geometries.TryGetValue(geomId, out var geometry)) continue;

                string nodeName = node.Attribute("name")?.Value ?? geomId;

                splineDataList.Add(new SplineData
                {
                    Name = nodeName,
                    Node = node,
                    Geometry = geometry,
                    Selected = true
                });
            }

            if (splineDataList.Count > 0)
            {
                SplineImporterWindow.ShowWindow(splineDataList);
            }
            else
            {
                EditorUtility.DisplayDialog("No Splines Found", "No splines were found in the selected file.", "OK");
            }
        }

        private static void CreateSplineGameObject(XElement node, XElement geometry, string nodeName)
        {
            GameObject go = new GameObject(nodeName);
            Undo.RegisterCreatedObjectUndo(go, "Import Splines");

            var container = go.AddComponent<SplineContainer>();
            Undo.RegisterCreatedObjectUndo(container, "Import Splines");

            if (container.Splines.Count > 0)
                container.RemoveSplineAt(0);

            var translateStr = node.Element("translate")?.Value;
            if (!string.IsNullOrEmpty(translateStr))
            {
                go.transform.position = ReadVec3(translateStr);
            }

            var rotateStr = node.Element("rotate")?.Value;
            if (!string.IsNullOrEmpty(rotateStr))
            {
                var q = ReadQuaternion(rotateStr);
                q.Normalize();
                var euler = q.eulerAngles;

                go.transform.rotation = Quaternion.Euler(euler.x, -euler.y, -euler.z);
            }

            var scaleStr = node.Element("scale")?.Value;
            if (!string.IsNullOrEmpty(scaleStr))
            {
                var s = ReadScale(scaleStr);
                go.transform.localScale = s;
            }

            if (nodeName.Contains(HEQuickstepTag))
                go.gameObject.tag = UnityQuickstepTag;

            if (nodeName.Contains(HESideViewTag))
                go.gameObject.tag = UnitySideViewTag;

            var splines = geometry.Descendants("spline3d");
            foreach (var splineNode in splines)
            {
                var spline = new Spline();
                Undo.RegisterCompleteObjectUndo(container, "Import Splines");
                container.AddSpline(spline);

                foreach (var knot in splineNode.Elements("knot"))
                {
                    Vector3 p = ReadVec3(knot.Element("point")?.Value);
                    Vector3 i = ReadVec3(knot.Element("invec")?.Value);
                    Vector3 o = ReadVec3(knot.Element("outvec")?.Value);

                    spline.Add(new BezierKnot(
                        p,
                        Vector3.zero,
                        Vector3.zero,
                        Quaternion.identity
                    ));
                }
            }

            EditorUtility.SetDirty(go);
        }

        private static Vector3 ReadVec3(string str)
        {
            if (string.IsNullOrEmpty(str)) return Vector3.zero;
            var parts = str.Split(' ');
            float x = -float.Parse(parts[0], CultureInfo.InvariantCulture);
            float y = float.Parse(parts[1], CultureInfo.InvariantCulture);
            float z = float.Parse(parts[2], CultureInfo.InvariantCulture);
            return new Vector3(x, y, z);
        }
        
        private static Quaternion ReadQuaternion(string str)
        {
            if (string.IsNullOrEmpty(str)) return Quaternion.identity;
            var parts = str.Split(' ');
            float x = float.Parse(parts[0], CultureInfo.InvariantCulture);
            float y = float.Parse(parts[1], CultureInfo.InvariantCulture);
            float z = float.Parse(parts[2], CultureInfo.InvariantCulture);
            float w = float.Parse(parts[3], CultureInfo.InvariantCulture);
            return new Quaternion(x, y, z, w);
        }

        private static Vector3 ReadScale(string str)
        {
            if (string.IsNullOrEmpty(str)) return Vector3.one;
            var parts = str.Split(' ');
            float x = float.Parse(parts[0], CultureInfo.InvariantCulture);
            float y = float.Parse(parts[1], CultureInfo.InvariantCulture);
            float z = float.Parse(parts[2], CultureInfo.InvariantCulture);
            return new Vector3(x, y, z);
        }
    }
}
