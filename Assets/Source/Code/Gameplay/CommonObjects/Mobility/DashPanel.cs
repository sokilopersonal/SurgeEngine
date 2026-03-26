using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.Inputs;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class DashPanel : StageObject
    {
        [SerializeField] private float speed = 35f;
        [SerializeField] private float outOfControl = 0.5f;

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);
            
            context.StateMachine.SetState<FStateGround>();
            Rigidbody body = context.Kinematics.Rigidbody;
            body.rotation = Quaternion.LookRotation(transform.forward, transform.up);
            var bodySpeed = context.Kinematics.Speed;
            if (bodySpeed < speed)
            {
                body.linearVelocity = transform.forward * speed;
            }
            else
            {
                body.linearVelocity = transform.forward * bodySpeed;
            }

            if (outOfControl > 0) context.Flags.AddFlag(new Flag(FlagType.OutOfControl, outOfControl));
            
            Rumble.Vibrate(0.7f, 0.9f, 0.5f);
        }

        private void OnDrawGizmosSelected()
        {
            Debug.DrawRay(transform.position, transform.forward * speed * outOfControl, Color.green);
        }
    }
}