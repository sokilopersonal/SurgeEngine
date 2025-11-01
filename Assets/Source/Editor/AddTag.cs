using UnityEditor;

namespace SurgeEngine.Source.Editor
{
    public static class AddTag
    {
        [MenuItem("GameObject/Surge Engine/Set Tags/Set As Concrete")]
        private static void SetAsConcrete()
        {
            var tag = Tags.Concrete;
            var selection = Selection.gameObjects;
            foreach (var obj in selection)
            {
                var name = obj.name;
                if (!name.Contains("@"))
                {
                    obj.name = $"{name}@{tag}";
                }
                else
                {
                    var split = name.Split('@');
                    obj.name = $"{split[0]}@{tag}";
                }
            }
                
            Undo.RecordObjects(selection, "Set As Concrete");
        }
        
        [MenuItem("GameObject/Surge Engine/Set Tags/Set As Grass")]
        private static void SetAsGrass()
        {
            var tag = Tags.Grass;
            var selection = Selection.gameObjects;
            foreach (var obj in selection)
            {
                var name = obj.name;
                if (!name.Contains("@"))
                {
                    obj.name = $"{name}@{tag}";
                }
                else
                {
                    var split = name.Split('@');
                    obj.name = $"{split[0]}@{tag}";
                }
            }
                
            Undo.RecordObjects(selection, "Set As Grass");
        }
        
        [MenuItem("GameObject/Surge Engine/Set Tags/Set As Water")]
        private static void SetAsWater()
        {
            var tag = Tags.Water;
            var selection = Selection.gameObjects;
            foreach (var obj in selection)
            {
                var name = obj.name;
                if (!name.Contains("@"))
                {
                    obj.name = $"{name}@{tag}";
                }
                else
                {
                    var split = name.Split('@');
                    obj.name = $"{split[0]}@{tag}";
                }
            }
                
            Undo.RecordObjects(selection, "Set As Water");
        }
    }

    enum Tags
    {
        Concrete,
        Grass,
        Water
    }
}