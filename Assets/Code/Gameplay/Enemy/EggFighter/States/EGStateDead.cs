using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.EggFighter.States
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
            eggFighter.animation.animator.enabled = false;
        }
    }
}