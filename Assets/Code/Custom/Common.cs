using UnityEngine;

namespace SurgeEngine.Code.Custom
{
    public static class Common
    {
        public static string GetGroundTag(this GameObject gameObject)
        {
            string input = gameObject.name;
            int index = input.IndexOf('@');
            string result = input.Substring(index + 1);
            
            return result;
        }

        public static bool InDelayTime(float last, float delay)
        {
            return last + delay < Time.time;
        }
        
        public static Vector3 GetCross(Transform transform, float pitch, bool inverse = false)
        {
            Vector3 cross = Vector3.Cross(Vector3.up, inverse ? transform.right : -transform.right);
            cross = Quaternion.AngleAxis(inverse ? pitch : -pitch, transform.right) * cross;
            return cross;
        }
    }
}