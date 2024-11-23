using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    public class Spinner : EnemyBase, IActor, IPlayerContactable
    {
        [SerializeField] private EnemyView view;
        
        public void InitializeComponents()
        {
            
        }

        public void OnContact()
        {
            ActorContext.Context.stateMachine.SetState<FStateAfterHoming>();
            Destroy(gameObject);
        }
    }
}