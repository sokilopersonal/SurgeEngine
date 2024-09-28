using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
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
            
            Common.ResetVelocity(ResetVelocityType.Both);
            
            _rigidbody.Sleep();
            animation.SetBool("Idle", true);
            if (stateMachine.PreviousState is not FStateStomp) animation.TransitionToState("Idle", 0.2f);
        }
        
        public override void OnExit()
        {
            base.OnExit();
            
            animation.SetBool("Idle", false);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (input.moveVector.magnitude > deadZone)
            {
                stateMachine.SetState<FStateGround>();
                _rigidbody.WakeUp();
            }

            if (stateMachine.GetSubState<FBoost>().Active)
            {
                _rigidbody.linearVelocity += _rigidbody.transform.forward * stateMachine.GetSubState<FBoost>().startForce;
                _rigidbody.WakeUp();
                stateMachine.SetState<FStateGround>();
            }

            if (!actor.flags.HasFlag(FlagType.OutOfControl))
            {
                if (actor.input.JumpPressed)
                {
                    actor.stateMachine.GetState<FStateGround>().SetDetachTime(0.2f);
                    actor.stateMachine.SetState<FStateJump>();
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Common.CheckForGround(out var hit))
            {
                var point = hit.point;
                var normal = hit.normal;

                _rigidbody.position = point + normal;
                Common.ResetVelocity(ResetVelocityType.Both);
                
                stats.transformNormal = stats.groundNormal;
            }
            else
            {
                stateMachine.SetState<FStateAir>();
            }
            
            stats.groundAngle = Vector3.Angle(stats.groundNormal, Vector3.up);
            if (stats.currentSpeed < 10 && stats.groundAngle >= 70)
            {
                _rigidbody.WakeUp();
                _rigidbody.AddForce(stats.groundNormal * 4f, ForceMode.Impulse);
            }
        }
    }
}