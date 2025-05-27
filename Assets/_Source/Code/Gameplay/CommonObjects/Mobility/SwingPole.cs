using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class SwingPole : ContactBase
    {
        [SerializeField] float shotVelSuccess;
        [SerializeField] float shotVelFail;
        [SerializeField] Transform grip;
        [SerializeField] bool trail2D;
        
        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);
            
            context.PutIn(grip.position);
            context.effects.SwingTrail.trail2D = trail2D;

            float lookDot = Vector3.Dot(context.transform.forward, transform.forward);
            float lookAngle = Vector3.Angle(context.transform.forward, transform.forward);

            //Debug.Log("Look Dot: " + lookDot);
            //Debug.Log("Look Angle: " + lookAngle);

            if (lookDot < 0f)
                grip.localEulerAngles = new Vector3(grip.localEulerAngles.x, 180, grip.localEulerAngles.z);
            else
                grip.localEulerAngles = new Vector3(grip.localEulerAngles.x, 0, grip.localEulerAngles.z);

            context.transform.rotation = grip.rotation;

            context.stateMachine.GetState<FStateSwingJump>().successVel = shotVelSuccess;
            context.stateMachine.GetState<FStateSwingJump>().failVel = shotVelFail;

            context.stateMachine.GetState<FStateSwing>().poleGrip = grip;
            context.stateMachine.SetState<FStateSwing>();
        }

        protected override void OnDrawGizmos()
        {
            TrajectoryDrawer.DrawTrajectory(transform.position, (transform.up + transform.forward).normalized, Color.green, shotVelSuccess);
            TrajectoryDrawer.DrawTrajectory(transform.position, (transform.up + transform.forward).normalized, Color.red, shotVelFail);
        }
    }
}
