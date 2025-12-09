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
    }
}