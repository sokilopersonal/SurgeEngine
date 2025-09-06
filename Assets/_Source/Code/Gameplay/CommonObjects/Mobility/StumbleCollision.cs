using FMODUnity;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public class StumbleCollision : StageObject
    {
        [Tooltip("Speed required to trigger the stumble")]
        [SerializeField] private float launchVelocity = 14;
        [SerializeField] private float noControlTime = 0.5f;
        [SerializeField] private EventReference stumbleSound;

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);
            
            if (context.Kinematics.Speed >= launchVelocity && context.Kinematics.CheckForGround(out _))
            {
                const float StumbleSpeed = 10;
                
                context.Rigidbody.linearVelocity = context.transform.forward * StumbleSpeed + 
                                                   context.transform.up * StumbleSpeed;
                
                context.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, noControlTime));
                if (context.StateMachine.GetState(out FBoost boost))
                {
                    boost.Active = false;
                }
                
                RuntimeManager.PlayOneShot(stumbleSound, context.transform.position);

                context.StateMachine.SetState<FStateStumble>().SetNoControlTime(noControlTime);
            }
        }
    }
}