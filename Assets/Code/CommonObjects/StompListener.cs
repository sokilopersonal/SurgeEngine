using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;
using UnityEngine.Events;
namespace SurgeEngine.Code.CommonObjects
{
    public class StompListener : ContactBase
    {
        public UnityEvent<Collider> onContact;
        public override void Contact(Collider msg)
        {
            base.Contact(msg);

            Actor context = ActorContext.Context;

            if (context.stateMachine.PreviousState is FStateStomp)
                onContact.Invoke(msg);
        }
    }
}
