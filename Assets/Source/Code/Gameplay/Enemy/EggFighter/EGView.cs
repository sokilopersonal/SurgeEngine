using FMODUnity;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter.States;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter
{
    public class EGView : EnemyView
    {
        private EggFighter eggFighter => (EggFighter)enemyBase;

        public override void Initialize(EnemyBase enemyBase)
        {
            base.Initialize(enemyBase);
            enemyBase.StateMachine.OnStateAssign += state =>
            {
                if (state is EGStateDead)
                    RuntimeManager.PlayOneShot(metalHitReference, transform.position);
            };   
        }

        public override void Load()
        {
            base.Load();

            eggFighter.StateMachine.SetState<EGStateIdle>();
            eggFighter.Animation.Animator.enabled = true;
        }
    }
}