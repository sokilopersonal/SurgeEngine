using SurgeEngine.Source.Code.Core.StateMachine;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.Base
{
    public abstract class FEState : FState
    {
        protected readonly FStateMachine StateMachine;
        protected readonly Transform Transform;

        protected FEState(EnemyBase enemy)
        {
            StateMachine = enemy.StateMachine;
            Transform = enemy.transform;
        }
    }
}