using UnityEngine;

namespace SurgeEngine.Code.Config.SonicSpecific
{
    [CreateAssetMenu(fileName = "DriftConfig", menuName = "SurgeEngine/Configs/Sonic/Drift", order = 0)]
    public class DriftConfig : ScriptableObject
    {
        public float centrifugalForce = 1.5f;
        public float minSpeed = 7f;
        public float maxSpeed = 2f;
    }
}