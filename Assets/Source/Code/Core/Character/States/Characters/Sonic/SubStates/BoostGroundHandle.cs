using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates
{
    public class BoostGroundHandle : IBoostHandler
    {
        public void BoostHandle(CharacterBase character, BoostConfig config)
        {
            float dt = Time.deltaTime;
            var body = character.Kinematics.Rigidbody;
            float speed = character.Kinematics.Speed;

            if (character.StateMachine.GetState(out FBoost boost) && boost.Active)
            {
                if (character.Input.MoveVector == Vector3.zero) character.Kinematics.SetInputDir(character.transform.forward);
                float maxSpeed = character.Config.topSpeed * config.TopSpeedMultiplier;
                if (speed < maxSpeed)
                {
                    body.AddForce(body.transform.forward * (config.Acceleration * dt), ForceMode.Impulse);
                }
                else
                {
                    if (character.Flags.HasFlag(FlagType.Autorun)) return;
                    
                    Vector3 target = body.linearVelocity.normalized * maxSpeed;
                    if (!body.isKinematic) body.linearVelocity = Vector3.MoveTowards(body.linearVelocity, target, 8f * dt);
                }
            }
        }
    }
}