using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.Character.System.Characters.Sonic;
using SurgeEngine.Source.Code.Infrastructure.Config.Sonic;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates
{
    public class BoostAirHandle : IBoostHandler
    {
        public void BoostHandle(CharacterBase character, BoostConfig config)
        {
            if (!character.Flags.HasFlag(FlagType.OutOfControl))
            {
                if (character.StateMachine.GetState(out FBoost boost) && character.Input.XPressed && boost.CanBoost() && boost.CanAirBoost)
                {
                    boost.CanAirBoost = false;
                    character.StateMachine.SetState<FStateAirBoost>();

                    var body = character.Rigidbody;
                    
                    Vector3 direction = Vector3.Cross(body.transform.right, Vector3.up);
                    Vector3 force = direction * config.AirBoostSpeed;

                    body.linearVelocity = force;
                    character.Model.RotateBody(Vector3.up);
                }
            }
        }
    }
}