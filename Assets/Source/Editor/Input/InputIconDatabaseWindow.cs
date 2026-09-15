#if UNITY_EDITOR

using System.Collections.Generic;
using SurgeEngine.Source.Code.Input;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace SurgeEngine.Source.Editor.Input
{
    public class InputIconDatabaseWindow : EditorWindow
    {
        private InputIconDatabase _database;

        private InputIconDatabase.DeviceType _deviceType =
            InputIconDatabase.DeviceType.Keyboard;

        private Vector2 _scroll;
        private string _search = string.Empty;

        private readonly List<ControlEntry> _controls = new();

        [MenuItem("Window/Input/Input Icon Database")]
        public static void Open()
        {
            GetWindow<InputIconDatabaseWindow>("Input Icons");
        }

        private void OnEnable()
        {
            ScanDevice();
        }

        private void OnFocus()
        {
            Repaint();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnGUI()
        {
            DrawHeader();

            if (_database == null)
            {
                EditorGUILayout.HelpBox(
                    "Assign an Input Icon Database.",
                    MessageType.Info);

                return;
            }

            EditorGUILayout.Space(5);

            DrawToolbar();

            EditorGUILayout.Space(5);

            DrawControls();
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField(
                "Input Icon Database",
                EditorStyles.boldLabel);

            EditorGUILayout.Space(5);

            InputIconDatabase previousDatabase = _database;

            _database = (InputIconDatabase)EditorGUILayout.ObjectField(
                "Database",
                _database,
                typeof(InputIconDatabase),
                false);

            if (previousDatabase != _database)
                ScanDevice();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal();

            InputIconDatabase.DeviceType previousDevice =
                _deviceType;

            _deviceType =
                (InputIconDatabase.DeviceType)EditorGUILayout.EnumPopup(
                    "Device",
                    _deviceType);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            string previousSearch = _search;

            _search = EditorGUILayout.TextField(
                "Search",
                _search);

            if (GUILayout.Button(
                    "Refresh",
                    GUILayout.Width(80)))
            {
                ScanDevice();
            }

            EditorGUILayout.EndHorizontal();

            if (previousDevice != _deviceType ||
                previousSearch != _search)
            {
                ScanDevice();
            }

            EditorGUILayout.Space(5);

            EditorGUILayout.HelpBox(
                "All controls available in the selected device layout are shown. " +
                "Sprites are read directly from the Input Icon Database.",
                MessageType.None);
        }

        private void ScanDevice()
        {
            _controls.Clear();

            string layoutName = GetDeviceLayout();

            if (string.IsNullOrEmpty(layoutName))
                return;

            InputControlLayout layout;

            try
            {
                layout = InputSystem.LoadLayout(layoutName);
            }
            catch
            {
                return;
            }

            HashSet<string> used = new();

            foreach (InputControlLayout.ControlItem controlItem in layout.controls)
            {
                if (string.IsNullOrEmpty(controlItem.name))
                    continue;

                if (!IsValidControl(controlItem))
                    continue;

                string path =
                    $"<{layoutName}>/{controlItem.name}";

                if (!MatchesSearch(
                        controlItem,
                        path))
                {
                    continue;
                }

                if (!used.Add(path))
                    continue;

                _controls.Add(new ControlEntry
                {
                    name = controlItem.name,
                    displayName = controlItem.displayName,
                    path = path
                });
            }

            Repaint();
        }

        private void DrawControls()
        {
            if (_controls.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No controls found.",
                    MessageType.Warning);

                return;
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            foreach (ControlEntry entry in _controls)
                DrawControl(entry);

            EditorGUILayout.EndScrollView();
        }

        private void DrawControl(ControlEntry entry)
        {
            Sprite currentSprite =
                _database.GetSprite(
                    entry.path,
                    _deviceType);

            EditorGUILayout.BeginHorizontal(
                EditorStyles.helpBox);

            EditorGUILayout.BeginVertical(
                GUILayout.Width(250));

            EditorGUILayout.LabelField(
                GetControlDisplayName(entry),
                EditorStyles.boldLabel);

            EditorGUILayout.LabelField(
                entry.path,
                EditorStyles.miniLabel);

            EditorGUILayout.EndVertical();

            Sprite sprite = (Sprite)EditorGUILayout.ObjectField(
                currentSprite,
                typeof(Sprite),
                false,
                GUILayout.Width(75),
                GUILayout.Height(75));

            if (sprite != currentSprite)
            {
                Undo.RecordObject(
                    _database,
                    "Change Input Icon");

                _database.SetSprite(
                    entry.path,
                    _deviceType,
                    sprite);

                EditorUtility.SetDirty(_database);
                AssetDatabase.SaveAssets();
            }

            if (currentSprite != null)
            {
                if (GUILayout.Button(
                        "Clear",
                        GUILayout.Width(50)))
                {
                    Undo.RecordObject(
                        _database,
                        "Clear Input Icon");

                    _database.Remove(
                        entry.path,
                        _deviceType);

                    EditorUtility.SetDirty(_database);
                    AssetDatabase.SaveAssets();
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(2);
        }

        private bool IsValidControl(
            InputControlLayout.ControlItem control)
        {
            string layout = control.layout;

            if (string.IsNullOrEmpty(layout))
                return true;

            return layout == "Button" ||
                   layout == "Key" ||
                   layout == "Axis" ||
                   layout == "Stick" ||
                   layout == "Dpad" ||
                   layout == "Vector2";
        }

        private bool MatchesSearch(
            InputControlLayout.ControlItem control,
            string path)
        {
            if (string.IsNullOrWhiteSpace(_search))
                return true;

            string search =
                _search.Trim().ToLowerInvariant();

            return control.name.ToString().ToLowerInvariant()
                       .Contains(search) ||
                   (control.displayName != null &&
                    control.displayName
                        .ToLowerInvariant()
                        .Contains(search)) ||
                   path.ToLowerInvariant()
                       .Contains(search);
        }

        private string GetDeviceLayout()
        {
            return _deviceType switch
            {
                InputIconDatabase.DeviceType.Keyboard =>
                    "Keyboard",

                InputIconDatabase.DeviceType.Mouse =>
                    "Mouse",

                InputIconDatabase.DeviceType.Xbox =>
                    "Gamepad",

                InputIconDatabase.DeviceType.PlayStation =>
                    "DualShockGamepad",

                _ => null
            };
        }

        private static string GetControlDisplayName(
            ControlEntry control)
        {
            if (!string.IsNullOrEmpty(control.displayName))
                return control.displayName;

            return control.name;
        }

        private class ControlEntry
        {
            public string name;
            public string displayName;
            public string path;
        }
    }
}

#endif