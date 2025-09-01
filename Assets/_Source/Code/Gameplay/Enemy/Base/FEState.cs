using SurgeEngine._Source.Code.Core.StateMachine;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.Enemy.Base
{
    public class FEState : FState
    {
        protected readonly FStateMachine StateMachine;
        protected readonly Transform transform;

        public FEState(EnemyBase enemy)
        {
            StateMachine = enemy.StateMachine;
            transform = enemy.transform;
        }
    }
}