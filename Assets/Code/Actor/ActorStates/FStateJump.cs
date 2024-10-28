using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.GameDocuments;
using UnityEngine;
using static SurgeEngine.Code.GameDocuments.SonicGameDocumentParams;

namespace SurgeEngine.Code.Parameters
{
    public class FStateJump : FStateAir
    {
        private float _jumpTime;
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            var doc = SonicGameDocument.GetDocument("Sonic");
            var param = doc.GetGroup(SonicGameDocument.PhysicsGroup);
            _rigidbody.AddForce(actor.transform.up * param.GetParameter<float>(BasePhysics_JumpForce), ForceMode.Impulse);
            _jumpTime = 0;
            
            actor.transform.rotation = Quaternion.Euler(0, actor.transform.rotation.eulerAngles.y, 0);
            
            actor.model.SetCollisionParam(param.GetParameter<float>(BasePhysics_JumpCollisionHeight), param.GetParameter<float>(BasePhysics_JumpCollisionCenter), param.GetParameter<float>(BasePhysics_JumpCollisionRadius));
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
                    var doc = SonicGameDocument.GetDocument("Sonic");
                    var param = doc.GetGroup(SonicGameDocument.PhysicsGroup);
                    if (_jumpTime < param.GetParameter<float>(BasePhysics_JumpStartTime))
                    {
                        if (_rigidbody.linearVelocity.y > 0) 
                            _rigidbody.linearVelocity += actor.transform.up * (param.GetParameter<float>(BasePhysics_JumpHoldForce) * dt);
                        _jumpTime += dt;
                    }
                }
            }
            
            stats.transformNormal = Vector3.up;

            Vector3 vel = _rigidbody.linearVelocity;
            vel = Vector3.ProjectOnPlane(vel, Vector3.up);

            if (vel.magnitude > 0.2f)
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