using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom.Extensions;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class StumbleCollision : StageObject
    {
        [Tooltip("Speed required to trigger the stumble")]
        [SerializeField] private float launchVelocity = 14;
        [SerializeField] private float noControlTime = 0.5f;
        [SerializeField] private EventReference stumbleSound;

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetCharacter(out CharacterBase character))
            {
                if (character.StateMachine.CurrentState is not FStateStumble && character.Kinematics.Speed >= launchVelocity && character.Kinematics.CheckForGround(out _))
                {
                    const float stumbleSpeed = 10;
                
                    character.Rigidbody.linearVelocity = character.transform.forward * stumbleSpeed + 
                                                       character.transform.up * stumbleSpeed;
                
                    character.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, noControlTime));
                    if (character.StateMachine.GetState(out FBoost boost))
                    {
                        boost.Active = false;
                    }
                
                    RuntimeManager.PlayOneShot(stumbleSound, character.transform.position);

                    character.StateMachine.SetState<FStateStumble>().SetNoControlTime(noControlTime);
                }
            }
        }
    }
}