using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateJump : FStateAir
    {
        public override void OnEnter()
        {
            base.OnEnter();
            
            animation.TransitionToState("Ball", 0, true);
            _rigidbody.AddForce(actor.transform.up * stats.jumpParameters.jumpForce, ForceMode.Impulse);
            
            actor.transform.rotation = Quaternion.Euler(0, actor.transform.rotation.eulerAngles.y, 0);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            stateMachine.GetState<FStateGround>().CalculateDetachState();

            if (stats.currentVerticalSpeed < -15f)
            {
                stateMachine.GetState<FStateGround>().SetAttachState(true);
                
                stateMachine.SetState<FStateAir>();
            }
        }
    }
}