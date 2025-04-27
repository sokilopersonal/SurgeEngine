using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.AeroCannon.States
{
    public class ACState : FEState
    {
        protected readonly AeroCannon aeroCannon;
        protected readonly AeroCannonConfig config;
        protected float timer;
        
        public ACState(EnemyBase enemy) : base(enemy)
        {
            aeroCannon = (AeroCannon)enemy;
            aeroCannon.TryGetConfig(out config);
        }
        
        protected bool IsInSight(out Transform target)
        {
            ActorBase context = ActorContext.Context;
            float viewDistance = config.viewDistance;
            LayerMask mask = config.mask;

            if (Vector3.Distance(context.transform.position, transform.position) < viewDistance)
            {
                bool result = UnityEngine.Physics.Linecast(transform.position, context.transform.position, mask);
                if (!result) // Make sure we see the player
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