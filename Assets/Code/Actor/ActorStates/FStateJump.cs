using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.GameDocuments;
using UnityEngine;
using static SurgeEngine.Code.GameDocuments.SonicGameDocumentParams;

namespace SurgeEngine.Code.Parameters
{
    public class FStateJump : FStateAir
    {
        private float _jumpTime;
        protected float _maxAirTime;

        public FStateJump(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _maxAirTime = 0.8f;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            var doc = SonicGameDocument.GetDocument("Sonic");
            var param = doc.GetGroup(SonicGameDocument.PhysicsGroup);
            _rigidbody.AddForce(Actor.transform.up * param.GetParameter<float>(BasePhysics_JumpForce), ForceMode.Impulse);
            _jumpTime = 0;
            
            Actor.transform.rotation = Quaternion.Euler(0, Actor.transform.rotation.eulerAngles.y, 0);
            
            Actor.model.SetCollisionParam(param.GetParameter<float>(BasePhysics_JumpCollisionHeight), param.GetParameter<float>(BasePhysics_JumpCollisionCenter), param.GetParameter<float>(BasePhysics_JumpCollisionRadius));
        }

        public override void OnExit()
        {
            Animation.ResetAction();
            Actor.model.SetCollisionParam(0,0);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (!Actor.flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.JumpHeld)
                {
                    var doc = SonicGameDocument.GetDocument("Sonic");
                    var param = doc.GetGroup(SonicGameDocument.PhysicsGroup);
                    if (_jumpTime < param.GetParameter<float>(BasePhysics_JumpStartTime))
                    {
                        if (_rigidbody.linearVelocity.y > 0) 
                            _rigidbody.linearVelocity += Actor.transform.up * (param.GetParameter<float>(BasePhysics_JumpHoldForce) * dt);
                        _jumpTime += dt;
                    }
                }
            }
            
            Kinematics.Normal = Vector3.up;

            if (GetAirTime() > _maxAirTime)
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}