using UnityEngine;

namespace SurgeEngine.Code.Config.SonicSpecific
{
    [CreateAssetMenu(fileName = "StompConfig", menuName = "SurgeEngine/Configs/Physics/Stomp", order = 0)]
    public class StompConfig : ScriptableObject
    {
        public float speed;
        public float slideSpeed = 5f;
        public AnimationCurve curve;
    }
}