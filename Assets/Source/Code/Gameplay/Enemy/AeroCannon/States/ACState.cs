using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.AeroCannon.States
{
    public class ACState : FEState
    {
        protected readonly AeroCannon aeroCannon;
        protected float timer;
        
        public ACState(EnemyBase enemy) : base(enemy)
        {
            aeroCannon = (AeroCannon)enemy;
        }
        
        protected bool IsInSight(out Transform target)
        {
            CharacterBase context = CharacterContext.Context;
            float viewDistance = aeroCannon.ViewDistance;
            LayerMask mask = aeroCannon.Mask;

            if (Vector3.Distance(context.transform.position, transform.position) < viewDistance)
            {
                bool result = UnityEngine.Physics.Linecast(transform.position, context.transform.position, mask);
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