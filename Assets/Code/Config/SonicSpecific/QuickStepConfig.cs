using UnityEngine;

namespace SurgeEngine.Code.Config.SonicSpecific
{
    [CreateAssetMenu(fileName = "QuickStepConfig", menuName = "SurgeEngine/Configs/Sonic/QuickStep", order = 0)]
    public class QuickStepConfig : ScriptableObject
    {
        public float duration = 0.15f;
        public float force = 36f;
        public float delay = 0.3f;
    }
}