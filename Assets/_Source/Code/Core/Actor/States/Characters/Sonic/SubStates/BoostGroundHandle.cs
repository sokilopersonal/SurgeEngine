using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Config;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates
{
    public class BoostGroundHandle : IBoostHandler
    {
        public void BoostHandle(ActorBase actor, BoostConfig config)
        {
            float dt = Time.deltaTime;
            BaseActorConfig baseConfig = actor.Config;
            Rigidbody body = actor.Kinematics.Rigidbody;
            float speed = actor.Kinematics.Speed;

            var boost = actor.StateMachine.GetSubState<FBoost>();
            if (boost.Active)
            {
                if (actor.Input.moveVector == Vector3.zero) actor.Kinematics.SetInputDir(actor.transform.forward);
                float maxSpeed = baseConfig.maxSpeed * config.MaxSpeedMultiplier;
                if (speed < maxSpeed)
                {
                    body.AddForce(body.linearVelocity.normalized * (config.Acceleration * dt), ForceMode.Impulse);
                }
            }
        }
    }
}