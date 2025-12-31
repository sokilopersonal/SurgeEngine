using JetBrains.Annotations;
using SurgeEngine.Source.Editor.HE1Importer;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace SurgeEngine.Source.Editor.ObjectSet
{
    [UsedImplicitly]
    public class SurgeToolbar
    {
        private const string AssetManagerButtonPath = "Surge Engine/Asset Manager";
        private const string TimeScaleSliderPath = "Surge Engine/Time Scale Slider";
        private const string TimeScaleResetPath = "Surge Engine/Time Scale Reset";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnSceneLoad()
        {
            MainToolbar.Refresh(TimeScaleSliderPath);
        }
        
        [MainToolbarElement(AssetManagerButtonPath, defaultDockPosition = MainToolbarDockPosition.Left)]
        public static MainToolbarElement AssetManagerButton()
        {
            var content = new MainToolbarContent("Asset Manager");
            return new MainToolbarButton(content, AssetManager.ShowWindow);
        }

        [MainToolbarElement(TimeScaleSliderPath, defaultDockPosition = MainToolbarDockPosition.Left)]
        public static MainToolbarElement TimeScaleSlider()
        {
            var content = new MainToolbarContent("Time Scale");
            var slider = new MainToolbarSlider(content, Time.timeScale, 0f, 5, OnTimeScaleChanged);

            slider.populateContextMenu = (menu) =>
            {
                menu.AppendAction("Reset", _ =>
                {
                    Time.timeScale = 1f;
                    MainToolbar.Refresh(TimeScaleSliderPath);
                });
            };
            
            return slider;
        }

        [MainToolbarElement(TimeScaleResetPath, defaultDockPosition = MainToolbarDockPosition.Left)]
        public static MainToolbarElement ResetTimeScaleButton()
        {
            var icon = EditorGUIUtility.IconContent("Refresh").image as Texture2D;
            var content = new MainToolbarContent(icon, "Reset time scale to 1");
            var button = new MainToolbarButton(content, () =>
            {
                Time.timeScale = 1f;
                MainToolbar.Refresh(TimeScaleSliderPath);
            });

            return button;
        }

        [MainToolbarElement("Surge Engine/HE1 Tools", defaultDockPosition = MainToolbarDockPosition.Left)]
        public static MainToolbarElement HE1ToolsDropdown()
        {
            var content = new MainToolbarContent("HE1 Tools");
            return new MainToolbarDropdown(content, ShowHE1ToolsDropdown);
        }

        private static void ShowHE1ToolsDropdown(Rect obj)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Import SetData XML"), false, HE1Tools.ImportXML);
            menu.AddItem(new GUIContent("Import Splines XML"), false, HE1Tools.ImportSplinesXML);
            menu.DropDown(obj);
        }

        private static void OnTimeScaleChanged(float obj)
        {
            Time.timeScale = obj;
        }
    }
}