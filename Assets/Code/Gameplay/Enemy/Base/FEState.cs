using SurgeEngine.Code.Core.StateMachine;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.Base
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