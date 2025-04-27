using SurgeEngine.Code.Core.Actor.States.SonicSpecific;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
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
