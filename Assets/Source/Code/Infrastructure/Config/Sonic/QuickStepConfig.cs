using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Config.Sonic
{
    [CreateAssetMenu(fileName = "QuickStepConfig", menuName = "Surge Engine/Configs/Sonic/QuickStep", order = 0)]
    public class QuickStepConfig : ScriptableObject
    {
        [Header("Quickstep")]
        public float duration = 0.15f;
        public float force = 36f;
        public float delay = 0.3f;

        [Header("Run Quickstep")]
        public float minSpeed = 5f;
        public float runDuration = 0.15f;
        public float runForce = 36f;
        public float runDelay = 0.3f;
    }
}