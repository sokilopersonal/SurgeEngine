using SurgeEngine.Code.Core.StateMachine;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.Base
{
    public class FEState : FState
    {
        protected readonly FStateMachine stateMachine;
        protected readonly Transform transform;

        public FEState(EnemyBase enemy)
        {
            stateMachine = enemy.StateMachine;
            transform = enemy.transform;
        }
    }
}