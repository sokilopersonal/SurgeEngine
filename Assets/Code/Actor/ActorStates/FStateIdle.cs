using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.GameDocuments;
using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
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
            
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            Common.ResetVelocity(ResetVelocityType.Both);

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
                
                Kinematics.Normal = hit.normal;
                Kinematics.Snap(hit.point, hit.normal, true);
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}