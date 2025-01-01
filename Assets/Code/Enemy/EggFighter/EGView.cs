using System;
using FMODUnity;
using SurgeEngine.Code.Enemy.States;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    public class EGView : EnemyView
    {
        protected override void ViewTick()
        {
            if (IsAbleExcludePlayer())
            {
                eCollider.excludeLayers = 1 << LayerMask.NameToLayer("Actor");
            }
            else
            {
                eCollider.excludeLayers = 0;
            }
        }

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