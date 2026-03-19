using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.AeroCannon.States
{
    public class ACStatePrepare : ACState
    {
        public ACStatePrepare(EnemyBase enemy, CharacterBase character) : base(enemy, character)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Timer = AeroCannon.PrepareTime;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (IsInSight(out var target))
            {
                Vector3 direction = target.position - Transform.position;
                direction.Normalize();
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                Transform.rotation = Quaternion.Slerp(Transform.rotation, rotation, 8f * Time.deltaTime);
                
                if (Utility.TickTimer(ref Timer, AeroCannon.PrepareTime, false))
                {
                    StateMachine.SetState<ACStateShoot>();
                }
            }
            else
            {
                StateMachine.SetState<ACStateIdle>();
            }
        }
    }
}