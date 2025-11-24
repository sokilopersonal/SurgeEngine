using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Config.Sonic
{
    [CreateAssetMenu(fileName = "LightSpeedDashConfig", menuName = "Surge Engine/Configs/Sonic/Light Speed Dash", order = 0)]
    public class LightSpeedDashConfig : ScriptableObject
    {
        [SerializeField] private float speed = 20;
        [SerializeField] private float radius = 5;
        [SerializeField] private float distance = 10;
        [SerializeField] private float offset = 0.5f;
        
        public float Speed => speed;
        public float Radius => radius;
        public float Distance => distance;
        public float Offset => offset;
    }
}