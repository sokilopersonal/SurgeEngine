using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateIdle : FCharacterState, IDamageableState
    {
        private float _angle;
        
        public FStateIdle(CharacterBase owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.Normal = Vector3.up;
            Rigidbody.linearVelocity = Vector3.zero;

            if (Kinematics.CheckForGround(out RaycastHit hit))
            {
                Kinematics.Snap(hit.point, Kinematics.Normal);
            }
        }

        public override void OnTick(float dt)
        {
            if (!Character.Flags.HasFlag(FlagType.OutOfControl))
            {
                if (Kinematics.GetInputDir().magnitude > 0.02f || Kinematics.HorizontalVelocity.magnitude > 0.02f)
                {
                    StateMachine.SetState<FStateGround>();
                }
                
                if (Input.APressed)
                {
                    Kinematics.SetDetachTime(0.1f);
                    StateMachine.SetState<FStateJump>();
                }
            }
            
            base.OnTick(dt);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Kinematics.CheckForGroundWithDirection(out RaycastHit hit, Vector3.down))
            {
                if (!Kinematics.CheckForPredictedGround(dt, Character.Config.castDistance, 4))
                {
                    StateMachine.SetState<FStateSlip>();
                }
                
                Quaternion target = Quaternion.FromToRotation(Rigidbody.transform.up, Vector3.up) * Rigidbody.rotation;
                Rigidbody.rotation = target;
                
                Kinematics.SlopePhysics();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}