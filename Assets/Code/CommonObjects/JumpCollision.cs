using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    /// <summary>
    /// Trigger for applying and clamping an impulse to the player
    /// </summary>
    public class JumpCollision : ActorTrigger
    {
        [Header("Properties")] 
        [SerializeField] private float speedRequired = 20f;
        [SerializeField] private float impulse = 15f;
        [SerializeField] private float outOfControl = 0.5f;

        protected override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            
            var context = ActorContext.Context;
            float dot = Vector3.Dot(transform.forward, context.transform.forward);
            if (dot > 0.5f) // Make sure the player is facing the same direction as the jump collision
            {
                if (context.stats.currentSpeed >= speedRequired) // If the player is moving fast enough apply an impulse
                {
                    if (impulse > 0)
                    {
                        context.AddImpulse(transform.forward * impulse);
                        context.rigidbody.linearVelocity = Vector3.ClampMagnitude(context.rigidbody.linearVelocity, impulse);
                    }
                    else if (impulse <= 0) // If the impulse is negative clamp the velocity
                    {
                        context.rigidbody.linearVelocity = Vector3.ClampMagnitude(context.rigidbody.linearVelocity, Mathf.Abs(impulse));
                    }
                
                    context.flags.AddFlag(new Flag(FlagType.OutOfControl, true, outOfControl));
                }
                else
                {
                    if (impulse <= 0)
                    {
                        context.rigidbody.linearVelocity = Vector3.ClampMagnitude(context.rigidbody.linearVelocity, speedRequired);
                    }
                }
            }
        }
    }
}