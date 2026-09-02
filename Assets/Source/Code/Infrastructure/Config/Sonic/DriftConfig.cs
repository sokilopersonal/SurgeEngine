using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Config.Sonic
{
    [CreateAssetMenu(fileName = "DriftConfig", menuName = "Surge Engine/Configs/Sonic/Drift", order = 0)]
    public class DriftConfig : ScriptableObject
    {
        public bool allowTriggerToActivate = true;
        public float centrifugalForce = 1.5f;
        public float maxSpeed = 2f;
    }
}