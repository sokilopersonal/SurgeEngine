using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateIdle : FActorState, IDamageableState
    {
        private float _angle;
        
        public FStateIdle(ActorBase owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.ResetVelocity();
        }

        public override void OnTick(float dt)
        {
            if (Kinematics.GetInputDir().magnitude > 0.02f || Kinematics.Speed > 0.02f)
            {
                StateMachine.SetState<FStateGround>();
            }

            if (!Actor.Flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.APressed)
                {
                    Kinematics.SetDetachTime(0.2f);
                    StateMachine.SetState<FStateJump>();
                }
            }
            
            base.OnTick(dt);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Kinematics.CheckForGroundWithDirection(out RaycastHit hit, Vector3.down, 2f))
            {
                Kinematics.Point = hit.point;
                Kinematics.Normal = Vector3.up;
                
                Vector3 forward = Vector3.Cross(Actor.transform.right, Vector3.up);
                Actor.transform.rotation = Quaternion.LookRotation(forward);
                Model.root.rotation = Quaternion.LookRotation(forward);
                
                Kinematics.Snap(Kinematics.Point, Kinematics.Normal, true);
                Kinematics.SlopePhysics();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}