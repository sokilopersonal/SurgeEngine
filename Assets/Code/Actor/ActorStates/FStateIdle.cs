using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.GameDocuments;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateIdle : FStateMove
    {
        [SerializeField, Range(0, 1)] private float deadZone;
        [SerializeField] private MoveParameters moveParameters;

        public FStateIdle(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Common.ResetVelocity(ResetVelocityType.Both);

        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            

            if (Input.moveVector.magnitude > deadZone)
            {
                StateMachine.SetState<FStateGround>();
            }

            if (StateMachine.GetSubState<FBoost>().Active)
            {
                _rigidbody.linearVelocity += _rigidbody.transform.forward * StateMachine.GetSubState<FBoost>().GetBoostEnergyGroup().GetParameter<float>(SonicGameDocumentParams.BoostEnergy_StartSpeed);
                StateMachine.SetState<FStateGround>();
            }

            if (!Actor.flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.JumpPressed)
                {
                    Kinematics.SetDetachTime(0.2f);
                    StateMachine.SetState<FStateJump>();
                }

                if (Input.BPressed)
                {
                    StateMachine.SetState<FStateSit>(0.1f);
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Common.CheckForGround(out var hit))
            {
                Actor.model.RotateBody(Kinematics.Normal);
                
                Kinematics.Snap(hit.point, hit.normal, true);
                Kinematics.Normal = hit.normal;
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}