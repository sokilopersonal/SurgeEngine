using UnityEngine;

namespace SurgeEngine.Code.Enemy.AeroCannon
{
    [CreateAssetMenu(fileName = "AeroCannonConfig", menuName = "SurgeEngine/Configs/Enemy/AeroCannon", order = 0)]
    public class AeroCannonConfig : ScriptableObject
    {
        public float viewDistance = 10;
        public LayerMask mask;
        public float prepareTime = 1;
    }
}