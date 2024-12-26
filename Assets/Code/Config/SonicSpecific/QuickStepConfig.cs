using UnityEngine;

namespace SurgeEngine.Code.Config.SonicSpecific
{
    [CreateAssetMenu(fileName = "QuickStepConfig", menuName = "SurgeEngine/Configs/Sonic/QuickStep", order = 0)]
    public class QuickStepConfig : ScriptableObject
    {
        public float quickStepDuration = 0.3f;
        public float quickStepForce = 15f;
    }
}