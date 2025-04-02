using SurgeEngine.Code.Actor.States.SonicSpecific;
using SurgeEngine.Code.Actor.System;
using UnityEngine;
using UnityEngine.Events;
namespace SurgeEngine.Code.CommonObjects
{
    public class StompListener : ContactBase
    {
        public UnityEvent onContact;
        
        public override void Contact(Collider msg)
        {
            base.Contact(msg);

            ActorBase context = ActorContext.Context;

            if (context.stateMachine.PreviousState is FStateStomp)
                onContact.Invoke();
        }
    }
}
