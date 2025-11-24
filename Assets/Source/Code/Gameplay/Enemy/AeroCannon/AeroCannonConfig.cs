using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.AeroCannon
{
    [CreateAssetMenu(fileName = "AeroCannonConfig", menuName = "Surge Engine/Configs/Enemy/AeroCannon", order = 0)]
    public class AeroCannonConfig : ScriptableObject
    {
        public float viewDistance = 10;
        public LayerMask mask;
        public float idleTime = 1.5f;
        public float prepareTime = 1;
    }
}