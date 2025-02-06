using UnityEngine;

namespace SurgeEngine.Code.Config
{
    [CreateAssetMenu(fileName = "DamageKick", menuName = "SurgeEngine/Config/DamageKick", order = 0)]
    public class DamageKickConfig : ScriptableObject
    {
        public float invincibleTime = 4f;
        public float directionalForce = 6f;
    }
}