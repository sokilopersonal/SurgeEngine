using SurgeEngine.Code.Enemy;
using UnityEngine;

namespace SurgeEngine.Code.StateMachine
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