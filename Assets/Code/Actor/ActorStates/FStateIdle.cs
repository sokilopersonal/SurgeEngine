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
            
            animation.SetBool("Idle", true);
            
            if (Physics.Raycast(actor.transform.position, -actor.transform.up, out var hit,
                    moveParameters.castParameters.castDistance, moveParameters.castParameters.collisionMask))
            {
                var point = hit.point;
                var normal = hit.normal;

                _rigidbody.position = point + normal;
            }
        }
        
        public override void OnExit()
        {
            base.OnExit();
            
            animation.SetBool("Idle", false);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _rigidbody.Sleep();

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
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Physics.Raycast(actor.transform.position, -actor.transform.up, out var hit,
                    moveParameters.castParameters.castDistance, moveParameters.castParameters.collisionMask))
            {
                var point = hit.point;
                var normal = hit.normal;

                _rigidbody.position = point + normal;
                
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