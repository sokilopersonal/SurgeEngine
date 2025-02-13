using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.AeroCannon.States
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
            Actor context = ActorContext.Context;
            float viewDistance = config.viewDistance;
            LayerMask mask = config.mask;

            if (Vector3.Distance(context.transform.position, transform.position) < viewDistance)
            {
                bool result = Physics.Linecast(transform.position, context.transform.position, mask);
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