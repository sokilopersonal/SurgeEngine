using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Config.SonicSpecific
{
    [CreateAssetMenu(fileName = "SlideConfig", menuName = "SurgeEngine/Configs/Sonic/Slide", order = 0)]
    public class SlideConfig : ScriptableObject
    {
        public float deceleration = 6f;
        public float minSpeed = 5f;
    }
}