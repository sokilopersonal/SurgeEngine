using FMODUnity;
using SurgeEngine.Code.Enemy.EggFighter.States;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.EggFighter
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
            eggFighter.animation.animator.enabled = true;
        }
    }
}