using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.AeroCannon.States
{
    public class ACStateShoot : ACState
    {
        public ACStateShoot(EnemyBase enemy, CharacterBase character) : base(enemy, character)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Timer = 1;

            if (IsInSight(out var target))
            {
                Vector3 direction = (target.position - Vector3.up * 0.5f) - Transform.position;
                direction.Normalize();

                var bullet = Object.Instantiate(AeroCannon.bulletPrefab, AeroCannon.shootPoint.position, Quaternion.identity);
                bullet.SetDirection(direction);
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Utility.TickTimer(ref Timer, 1, false))
            {
                StateMachine.SetState<ACStateIdle>();
            }
        }
    }
}