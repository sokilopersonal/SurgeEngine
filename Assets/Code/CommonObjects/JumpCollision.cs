using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.SurgeDebug;
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
        [SerializeField] private float pitch = 10f;
        [SerializeField] private float impulseOnNormal = 15f;
        [SerializeField] private float impulseOnBoost = 15f;
        [SerializeField] private float outOfControl = 0.5f;

        protected override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            var context = ActorContext.Context;
            float dot = Vector3.Dot(context.transform.forward, context.transform.forward);
            float impulse = context.stateMachine.GetSubState<FBoost>().Active ? impulseOnBoost : impulseOnNormal;
            Vector3 cross = GetCross(pitch);
            
            if (dot > 0) // Make sure the player is facing the same direction as the jump collision
            {
                if (context.stats.currentSpeed >= speedRequired) // If the player is moving fast enough apply an impulse
                {
                    if (impulse > 0)
                    {
                        context.rigidbody.linearVelocity = Vector3.zero;
                        context.stats.planarVelocity = Vector3.zero;
                        context.stats.movementVector = Vector3.zero;

                        context.stats.transformNormal = GetCross(0f);

                        Vector3 impulseV = cross.normalized * impulse;
                        
                        context.AddImpulse(impulseV);
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

        private Vector3 GetCross(float pitch)
        {
            Vector3 cross = Vector3.Cross(Vector3.up, -transform.right);
            cross = Quaternion.AngleAxis(-pitch, transform.right) * cross;
            return cross;
        }

        protected override void Draw()
        {
            base.Draw();

            TrajectoryDrawer.DrawTrajectoryWithTwoImpulses(transform.position, GetCross(pitch), impulseOnNormal, impulseOnBoost);
        }

    }
}
