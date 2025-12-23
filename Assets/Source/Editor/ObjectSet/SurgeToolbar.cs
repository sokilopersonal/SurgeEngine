using JetBrains.Annotations;
using SurgeEngine.Source.Editor.HE1Importer;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace SurgeEngine.Source.Editor.ObjectSet
{
    [UsedImplicitly]
    class SurgeToolbar
    {
        [MainToolbarElement("Surge Engine/Asset Manager", defaultDockPosition = MainToolbarDockPosition.Left)]
        public static MainToolbarElement AssetManagerButton()
        {
            var content = new MainToolbarContent("Asset Manager");
            return new MainToolbarButton(content, ObjectSet.ShowWindow);
        }

        [MainToolbarElement("Surge Engine/Time Scale Slider", defaultDockPosition = MainToolbarDockPosition.Left)]
        public static MainToolbarElement TimeScaleSlider()
        {
            var content = new MainToolbarContent("Time Scale");
            return new MainToolbarSlider(content, Time.timeScale, 0f, 2, OnTimeScaleChanged);
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