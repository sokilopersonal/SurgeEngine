using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.GameDocuments;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateJump : FStateAir
    {
        private float _jumpTime;
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            _rigidbody.AddForce(actor.transform.up * stats.jumpParameters.jumpForce, ForceMode.Impulse);
            _jumpTime = 0;
            
            actor.transform.rotation = Quaternion.Euler(0, actor.transform.rotation.eulerAngles.y, 0);
            
            var doc = SonicGameDocument.GetDocument("Sonic");
            var param = doc.GetGroup(SonicGameDocument.PhysicsGroup);
            actor.model.SetCollisionParam(param.GetParameter<float>("JumpCollisionHeight"), param.GetParameter<float>("JumpCollisionCenter"), param.GetParameter<float>("JumpCollisionRadius"));
        }

        public override void OnExit()
        {
            base.OnExit();
            
            animation.ResetAction();
            actor.model.SetCollisionParam(0,0);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (!actor.flags.HasFlag(FlagType.OutOfControl))
            {
                if (input.JumpHeld)
                {
                    if (_jumpTime < stats.jumpParameters.jumpStartTime)
                    {
                        if (_rigidbody.linearVelocity.y > 0) 
                            _rigidbody.linearVelocity += actor.transform.up * (stats.jumpParameters.jumpHoldForce * dt);
                        _jumpTime += dt;
                    }
                }
            }
            
            stats.transformNormal = Vector3.up;

            Vector3 vel = _rigidbody.linearVelocity;
            vel = Vector3.ProjectOnPlane(vel, Vector3.up);

            if (vel.magnitude > 0.5f)
            {
                Quaternion rot = Quaternion.LookRotation(vel, stats.transformNormal);
                actor.transform.rotation = rot;
            }

            if (GetAirTime() > 0.85f)
            {
                stateMachine.SetState<FStateAir>();
            }
        }
    }
}