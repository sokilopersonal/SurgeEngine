using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates
{
    public class BoostAirHandle : IBoostHandler
    {
        public void BoostHandle(ActorBase actor, BoostConfig config)
        {
            if (!actor.Flags.HasFlag(FlagType.OutOfControl))
            {
                FBoost boost = actor.StateMachine.GetSubState<FBoost>();
                if (actor.Input.XPressed && boost.CanBoost() && boost.CanAirBoost)
                {
                    boost.CanAirBoost = false;
                    actor.StateMachine.SetState<FStateAirBoost>();

                    var body = actor.Rigidbody;
                    
                    Vector3 direction = Vector3.Cross(body.transform.right, Vector3.up);
                    Vector3 force = direction * config.AirBoostSpeed;

                    body.linearVelocity = force;
                    actor.Model.RotateBody(Vector3.up);
                }
            }
        }
    }
}