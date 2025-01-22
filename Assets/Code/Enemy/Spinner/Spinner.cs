using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    public class Spinner : EnemyBase, IDamageable
    {
        [SerializeField] private EnemyView view;

        public void TakeDamage(object sender, float damage)
        {
            view.Destroy();
        }
    }
}