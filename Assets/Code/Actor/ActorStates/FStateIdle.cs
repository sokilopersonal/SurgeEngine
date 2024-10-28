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

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (Common.CheckForGround(out var hit))
            {
                var point = hit.point;
                var normal = hit.normal;

                _rigidbody.position = point + normal;
                stats.transformNormal = stats.groundNormal;
            }
            
            _rigidbody.Sleep();
        }
        
        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            Common.ResetVelocity(ResetVelocityType.Both);

            if (!Common.CheckForGround(out _))
            {
                stateMachine.SetState<FStateAir>();
            }

            if (input.moveVector.magnitude > deadZone)
            {
                stateMachine.SetState<FStateGround>();
                _rigidbody.WakeUp();
            }

            if (stateMachine.GetSubState<FBoost>().Active)
            {
                _rigidbody.linearVelocity += _rigidbody.transform.forward * stateMachine.GetSubState<FBoost>().GetBoostEnergyGroup().GetParameter<float>(SonicGameDocumentParams.BoostEnergy_StartSpeed);
                _rigidbody.WakeUp();
                stateMachine.SetState<FStateGround>();
            }

            if (!actor.flags.HasFlag(FlagType.OutOfControl))
            {
                if (input.JumpPressed)
                {
                    stateMachine.GetState<FStateGround>().SetDetachTime(0.2f);
                    stateMachine.SetState<FStateJump>();
                }

                if (input.BPressed)
                {
                    stateMachine.SetState<FStateSit>(0.1f);
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            stats.groundAngle = Vector3.Angle(stats.groundNormal, Vector3.up);
            if (stats.currentSpeed < 10 && stats.groundAngle >= 70)
            {
                _rigidbody.WakeUp();
                _rigidbody.AddForce(stats.groundNormal * 4f, ForceMode.Impulse);
            }
        }
    }
}