using FMODUnity;
using SurgeEngine.Code.Gameplay.Enemy.Base;
using SurgeEngine.Code.Gameplay.Enemy.EggFighter.States;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter
{
    public class EGView : EnemyView
    {
        private EggFighter eggFighter => (EggFighter)enemyBase;
        
        private void OnEnable()
        {
            enemyBase.stateMachine.OnStateAssign += state =>
            {
                if (state is EGStateDead)
                    RuntimeManager.PlayOneShot(metalHitReference, transform.position);
            };   
        }

        public override void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            base.Load(loadPosition, loadRotation);

            eggFighter.stateMachine.SetState<EGStateIdle>();
            eggFighter.animation.Animator.enabled = true;
        }
    }
}