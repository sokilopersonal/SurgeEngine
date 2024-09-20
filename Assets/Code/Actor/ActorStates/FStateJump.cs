using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateJump : FStateAir
    {
        public override void OnEnter()
        {
            base.OnEnter();
            
            animation.TransitionToState("Ball", 0, true);
            //_rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
            _rigidbody.AddForce(actor.transform.up * stats.jumpParameters.jumpForce, ForceMode.Impulse);
            
            actor.transform.rotation = Quaternion.Euler(0, actor.transform.rotation.eulerAngles.y, 0);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            stats.transformNormal = Vector3.up;

            Vector3 vel = _rigidbody.linearVelocity;
            vel = Vector3.ProjectOnPlane(vel, Vector3.up);

            if (vel.magnitude > 0.1f)
            {
                Quaternion rot = Quaternion.LookRotation(vel, stats.transformNormal);
                actor.transform.rotation = rot;
            }

            if (GetAirTime() > 1f && stats.currentVerticalSpeed < -15f)
            {
                stateMachine.SetState<FStateAir>();
            }
        }
    }
}