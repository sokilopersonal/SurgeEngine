using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.Inputs;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public class DashPanel : StageObject
    {
        [SerializeField] private float speed = 35f;
        [SerializeField] private float outOfControl = 0.5f;

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);
            
            Rigidbody body = context.Kinematics.Rigidbody;
            var bodySpeed = context.Kinematics.Speed;
            if (bodySpeed < speed)
            {
                body.linearVelocity = transform.forward * speed;
            }
            else
            {
                body.linearVelocity = transform.forward * bodySpeed;
            }
            
            context.StateMachine.SetState<FStateGround>();
            
            body.rotation = Quaternion.LookRotation(transform.forward, transform.up);
            context.Model.root.rotation = body.rotation;

            context.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, Mathf.Abs(outOfControl)));
            
            new Rumble().Vibrate(0.7f, 0.9f, 0.5f);
        }

        private void OnDrawGizmosSelected()
        {
            Debug.DrawRay(transform.position, transform.forward * speed * outOfControl, Color.green);
        }
    }
}