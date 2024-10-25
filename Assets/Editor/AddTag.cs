using UnityEditor;
using UnityEngine;

namespace SurgeEngine.Editor
{
    public class AddTag : EditorWindow
    {
        private Tags _tag;
        
        [MenuItem("Surge Engine/Tag Window")]
        private static void ShowWindow()
        {
            var window = GetWindow<AddTag>();
            window.titleContent = new GUIContent("Tag Adder");
            window.minSize = new Vector2(300, 180);
            window.Show();
        }

        private void OnGUI()
        {
            _tag = (Tags)EditorGUILayout.EnumPopup("Tag", _tag);
            
            if (GUILayout.Button("Add Tag"))
            {
                Undo.RecordObjects(Selection.gameObjects, "Tagged Objects");
                
                foreach (var obj in Selection.gameObjects)
                {
                    var name = obj.name;
                    if (!name.Contains("@"))
                    {
                        obj.name = $"{name}@{_tag}";
                    }
                    else
                    {
                        var split = name.Split('@');
                        obj.name = $"{split[0]}@{_tag}";
                    }
                }
            }
        }
    }

    enum Tags
    {
        Concrete,
        Grass,
        Water
    }
}