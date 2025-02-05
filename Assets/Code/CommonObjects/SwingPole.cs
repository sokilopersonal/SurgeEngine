using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CustomDebug;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class SwingPole : ContactBase
    {
        public float shotVelSuccess;
        public float shotVelFail;
        public Transform grip;
        public bool trail2D = false;
        
        public override void Contact(Collider msg)
        {
            base.Contact(msg);

            Actor context = ActorContext.Context;

            context.transform.position = grip.position;

            context.effects.swingTrail.trail2D = trail2D;

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
