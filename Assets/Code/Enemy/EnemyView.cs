using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;

namespace SurgeEngine.Code.StateMachine
{
    public abstract class EnemyView : MonoBehaviour, IEnemyComponent
    {
        public EnemyBase enemyBase { get; set; }
        
        public CapsuleCollider eCollider;

        private void Update()
        {
            ViewTick();
        }

        protected abstract void ViewTick();

        protected bool IsAbleExcludePlayer()
        {
            return enemyBase.CanBeDamaged();
        }
    }
}