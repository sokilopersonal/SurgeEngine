using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
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
            
            Kinematics.ResetVelocity();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (Kinematics.GetInputDir().magnitude > 0.1f)
            {
                StateMachine.SetState<FStateGround>();
            }

            if (!character.Flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.APressed)
                {
                    Kinematics.SetDetachTime(0.1f);
                    StateMachine.SetState<FStateJump>();
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Kinematics.CheckForGroundWithDirection(out RaycastHit hit, Vector3.down, 2f))
            {
                Kinematics.Point = hit.point;
                Kinematics.Normal = Vector3.up;
                
                if (!Kinematics.CheckForPredictedGround(dt, character.Config.castDistance, 4))
                {
                    StateMachine.SetState<FStateSlip>();
                }
                
                Quaternion target = Quaternion.FromToRotation(Rigidbody.transform.up, Vector3.up) * Rigidbody.rotation;
                Rigidbody.rotation = Quaternion.Lerp(Rigidbody.rotation, target, Time.fixedDeltaTime * 8f);
                
                Kinematics.Snap(Kinematics.Point, Vector3.up);
                Kinematics.SlopePhysics();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}