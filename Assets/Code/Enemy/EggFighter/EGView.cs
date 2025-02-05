using FMODUnity;
using SurgeEngine.Code.Enemy.EggFighter.States;

namespace SurgeEngine.Code.Enemy.EggFighter
{
    public class EGView : EnemyView
    {
        private void OnEnable()
        {
            enemyBase.stateMachine.OnStateAssign += state =>
            {
                if (state is EGStateDead)
                    RuntimeManager.PlayOneShot(metalHitReference, transform.position);
            };   
        }
    }
}