using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.GameDocuments;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;
using static SurgeEngine.Code.GameDocuments.SonicGameDocumentParams;

namespace SurgeEngine.Code.Parameters
{
    public class FStateHoming : FStateMove
    {
        private Transform _target;
        private float _timer;

        public FStateHoming(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            StateMachine.GetSubState<FBoost>().Active = false;
            Stats.transformNormal = Vector3.up;

            _timer = 0f;
            
            var doc = SonicGameDocument.GetDocument("Sonic");
            var param = doc.GetGroup(SonicGameDocument.PhysicsGroup);
            Actor.model.SetCollisionParam(param.GetParameter<float>("JumpCollisionHeight"), param.GetParameter<float>("JumpCollisionCenter"), param.GetParameter<float>("JumpCollisionRadius"));

            Common.ResetVelocity(ResetVelocityType.Both);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            Animation.ResetAction();
            Actor.model.SetCollisionParam(0, 0);
            _target = null;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            var param = SonicGameDocument.GetDocument("Sonic").GetGroup(SonicGameDocument.HomingGroup);
            
            if (_target != null)
            {
                Vector3 direction = (_target.position - Actor.transform.position).normalized;
                _rigidbody.linearVelocity = direction * param.GetParameter<float>(Homing_Speed);
                _rigidbody.rotation = Quaternion.LookRotation(direction, Vector3.up);
                
                // If for some reason Sonic get stuck
                _timer += dt / param.GetParameter<float>(Homing_MaxTargetTime);
                if (_timer >= 1f)
                {
                    StateMachine.SetState<FStateAir>();
                }
            }
            else
            {
                if (Common.CheckForGround(out _, CheckGroundType.PredictJump))
                {
                    StateMachine.SetState<FStateAir>();
                }
                
                Vector3 direction = Actor.transform.forward;
                _rigidbody.linearVelocity = direction * (param.GetParameter<float>(Homing_Distance) *
                                                         param.GetParameter<AnimationCurve>(Homing_Curve).Evaluate(_timer));
                _rigidbody.rotation = Quaternion.LookRotation(direction, Vector3.up);
                
                _timer += dt / param.GetParameter<float>(Homing_Time);
                if (_timer >= 1f)
                {
                    StateMachine.SetState<FStateAir>();
                }
            }
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }
    }
}