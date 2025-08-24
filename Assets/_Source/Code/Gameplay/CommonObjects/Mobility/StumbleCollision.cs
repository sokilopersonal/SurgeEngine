using FMODUnity;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class StumbleCollision : ContactBase
    {
        [Tooltip("Speed required to trigger the stumble")]
        [SerializeField] private float launchVelocity = 14;
        [SerializeField] private float noControlTime = 0.5f;
        [SerializeField] private EventReference stumbleSound;

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            if (context.Kinematics.Speed >= launchVelocity)
            {
                const float StumbleSpeed = 11;
                
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