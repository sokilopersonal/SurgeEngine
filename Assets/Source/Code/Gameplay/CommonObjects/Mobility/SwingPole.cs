using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom.Drawers;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class SwingPole : StageObject
    {
        [SerializeField] private float shotVelSuccess = 17;
        [SerializeField] private float shotVelFail = 10;
        [SerializeField] private Transform grip;
        [SerializeField] private bool trail2D;
        
        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            context.Rigidbody.position = grip.position;
            context.Effects.SwingTrail.trail2D = trail2D;

            float lookDot = Vector3.Dot(context.transform.forward, transform.forward);

            if (lookDot < 0f)
                grip.localEulerAngles = new Vector3(grip.localEulerAngles.x, 180, grip.localEulerAngles.z);
            else
                grip.localEulerAngles = new Vector3(grip.localEulerAngles.x, 0, grip.localEulerAngles.z);

            context.transform.rotation = grip.rotation;

            var state = context.StateMachine.GetState<FStateSwingJump>();
            state.successVel = shotVelSuccess;
            state.failVel = shotVelFail;

            context.StateMachine.GetState<FStateSwing>().poleGrip = grip;
            context.StateMachine.SetState<FStateSwing>();
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(transform.position, (transform.up + transform.forward).normalized, Color.green, shotVelSuccess);
            TrajectoryDrawer.DrawTrajectory(transform.position, (transform.up + transform.forward).normalized, Color.red, shotVelFail);
        }
    }
}
