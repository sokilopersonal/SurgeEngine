using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    public class FEState : FState
    {
        protected EnemyBase enemy;
        protected FStateMachine stateMachine;
        protected Transform transform;

        public FEState(EnemyBase enemy)
        {
            this.enemy = enemy;
            stateMachine = enemy.stateMachine;
            transform = enemy.transform;
        }
    }
}