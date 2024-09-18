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

            if (actor.input.moveVector.magnitude > deadZone)
            {
                stateMachine.SetState<FStateGround>();
                _rigidbody.WakeUp();
            }

            if (actor.input.BoostPressed)
            {
                _rigidbody.linearVelocity += _rigidbody.transform.forward * actor.stats.boost.startForce;
                _rigidbody.WakeUp();
                if (Physics.Raycast(actor.transform.position, -actor.transform.up, out var hit,
                        moveParameters.castParameters.castDistance, moveParameters.castParameters.collisionMask))
                {
                    var point = hit.point;
                    var normal = hit.normal;

                    _rigidbody.position = point + normal;
                }
                stateMachine.SetState<FStateGround>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (!Physics.Raycast(actor.transform.position, -actor.transform.up, out _,
                    moveParameters.castParameters.castDistance, moveParameters.castParameters.collisionMask))
            {
                stateMachine.SetState<FStateAir>();
            }
        }
    }
}