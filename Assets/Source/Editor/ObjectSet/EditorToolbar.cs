using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace SurgeEngine.Source.Editor.ObjectSet
{
    class EditorToolbar
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

        private static void OnTimeScaleChanged(float obj)
        {
            Time.timeScale = obj;
        }
    }
}