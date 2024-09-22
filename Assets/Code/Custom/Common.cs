using UnityEngine;

namespace SurgeEngine.Code.Custom
{
    public static class Common
    {
        public static bool CheckForGroundTag(this GameObject gameObject, string tag)
        {
            if (tag.Contains("@"))
                tag = tag.Replace("@", "");
            
            return gameObject.name.Contains($"@{tag}");
        }
        
        public static string GetGroundTag(this GameObject gameObject)
        {
            string input = gameObject.name;
            int index = input.IndexOf('@');
            if (index >= 0 && index < input.Length - 1)
            {
                string result = input.Substring(index + 1);
                return result;
            }

            return "Concrete"; // If there is no tag, use "Concrete" as a default
        }

        public static bool InDelayTime(float last, float delay)
        {
            return last + delay < Time.time;
        }

    }
}