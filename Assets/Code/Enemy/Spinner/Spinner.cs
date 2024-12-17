using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    public class Spinner : EnemyBase, IPlayerContactable
    {
        [SerializeField] private EnemyView view;

        public void OnContact()
        {
            ActorContext.Context.stateMachine.SetState<FStateAfterHoming>();
            view.Destroy();
        }
    }
}