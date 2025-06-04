using SurgeEngine.Code.Gameplay.Enemy.Base;
using SurgeEngine.Code.Gameplay.Enemy.RagdollPhysics;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter.States
{
    public class EGStateDead : EGState
    {
        private float _timer;
        
        public EGStateDead(EnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public void ApplyKnockback(Vector3 force, EnemyRagdoll ragdoll)
        {
            ragdoll.Ragdoll(force);
            eggFighter.animation.Animator.enabled = false;
        }
    }
}