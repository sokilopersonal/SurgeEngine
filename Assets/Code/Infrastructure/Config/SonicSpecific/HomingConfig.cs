using NaughtyAttributes;
using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Config.SonicSpecific
{
    [CreateAssetMenu(fileName = "HomingConfig", menuName = "SurgeEngine/Configs/Sonic/Homing", order = 0)]
    public class HomingConfig : ScriptableObject
    {
        [Foldout("Homing")] public float speed = 35;
        [Foldout("Homing")] public float findRadius = 5.5f;
        [Foldout("Homing")] public float findDistance = 16f;
        [Foldout("Homing")] public float maxTime = 1.5f;
        [Foldout("Homing")] public float afterForce = 10f;
        [Foldout("Homing")] public float delay = 0.1f;
        [Foldout("Homing")] public LayerMask mask;
        
        [Foldout("Jump Dash")] public float jumpDashTime = 0.45f;
        [Foldout("Jump Dash")] public float jumpDashDistance = 25;
        [Foldout("Jump Dash")] public AnimationCurve JumpDashCurve;
    }
}