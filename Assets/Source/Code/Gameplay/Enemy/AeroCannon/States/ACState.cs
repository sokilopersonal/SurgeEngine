using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.AeroCannon.States
{
    public abstract class ACState : FEState
    {
        protected readonly AeroCannon AeroCannon;
        protected readonly CharacterBase Character;
        protected float Timer;

        protected ACState(EnemyBase enemy, CharacterBase character) : base(enemy)
        {
            AeroCannon = (AeroCannon)enemy;
            Character = character;
        }
        
        protected bool IsInSight(out Transform target)
        {
            CharacterBase context = Character;
            float viewDistance = AeroCannon.ViewDistance;
            LayerMask mask = AeroCannon.Mask;

            if (Vector3.Distance(context.transform.position, Transform.position) < viewDistance)
            {
                bool result = Physics.Linecast(Transform.position, context.transform.position, mask);
                if (!result)
                {
                    target = context.transform;
                    return true;
                }
            }

            target = null;
            return false;
        }
    }
}