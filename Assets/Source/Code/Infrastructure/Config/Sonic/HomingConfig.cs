
using Alchemy.Inspector;
using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Config.Sonic
{
    [CreateAssetMenu(fileName = "HomingConfig", menuName = "Surge Engine/Configs/Sonic/Homing", order = 0)]
    public class HomingConfig : ScriptableObject
    {
        [FoldoutGroup("Homing")] public float speed = 35;
        [FoldoutGroup("Homing")] public float findDistance = 16f;
        [FoldoutGroup("Homing")] public float findAngle = 55f;
        [FoldoutGroup("Homing")] public float maxTime = 1.5f;
        [FoldoutGroup("Homing")] public float afterForce = 10f;
        [FoldoutGroup("Homing")] public float delay = 0.1f;
        [FoldoutGroup("Homing")] public LayerMask mask;
        
        [FoldoutGroup("Jump Dash")] public float jumpDashTime = 0.45f;
        [FoldoutGroup("Jump Dash")] public float jumpDashDistance = 25;
        [FoldoutGroup("Jump Dash")] public AnimationCurve JumpDashCurve;
    }
}