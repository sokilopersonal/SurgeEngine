using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateIdle : FStateMove
    {
        [SerializeField, Range(0, 1)] private float deadZone;

        public override void OnEnter()
        {
            base.OnEnter();
            
            actor.animation.SetBool("Idle", true);
        }
        
        public override void OnExit()
        {
            base.OnExit();
            
            actor.animation.SetBool("Idle", false);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _rigidbody.Sleep();

            if (actor.input.moveVector.magnitude > deadZone)
            {
                actor.stateMachine.SetState<FStateGround>();
            }
        }
    }
}