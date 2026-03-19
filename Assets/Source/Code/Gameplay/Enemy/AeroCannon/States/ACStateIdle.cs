using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.AeroCannon.States
{
    public class ACStateIdle : ACState
    {
        private Quaternion _startRotation;
        
        public ACStateIdle(EnemyBase enemy, CharacterBase character) : base(enemy, character)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Timer = AeroCannon.IdleTime;
        }

        public override void OnTick(float dt)
        {
            bool inSight = IsInSight(out var target);
            if (Utility.TickTimer(ref Timer, AeroCannon.IdleTime, false))
            {
                if (inSight)
                {
                    StateMachine.SetState<ACStatePrepare>();
                }
            }

            if (inSight)
            {
                Vector3 direction = target.position - Transform.position;
                direction.Normalize();
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                Transform.rotation = Quaternion.Slerp(Transform.rotation, rotation, 10f * Time.deltaTime);
            }
            else
            {
                Transform.rotation = Quaternion.Slerp(Transform.rotation, _startRotation, 2f * Time.deltaTime);
            }
        }
        
        public void SetStartRotation(Quaternion startRotation) => _startRotation = startRotation;
    }
}