using System;
using FMODUnity;
using SurgeEngine.Code.Enemy.States;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    public class EGView : EnemyView
    {
        private void OnEnable()
        {
            enemyBase.stateMachine.OnStateAssign += state =>
            {
                if (state is EGStateDead)
                    RuntimeManager.PlayOneShot(metalHitReference, transform.position);
            };   
        }
    }
}