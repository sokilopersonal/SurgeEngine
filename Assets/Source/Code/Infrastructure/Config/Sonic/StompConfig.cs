using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Config.Sonic
{
    [CreateAssetMenu(fileName = "StompConfig", menuName = "Surge Engine/Configs/Physics/Stomp", order = 0)]
    public class StompConfig : ScriptableObject
    {
        public float speed;
        public float slideSpeed = 5f;
        public AnimationCurve curve;
    }
}