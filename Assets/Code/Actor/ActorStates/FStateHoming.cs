using SurgeEngine.Code.Custom;
using SurgeEngine.Code.GameDocuments;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateHoming : FStateMove
    {
        private Transform _target;
        private float _timer;
        
        public override void OnEnter()
        {
            base.OnEnter();

            stateMachine.GetSubState<FBoost>().Active = false;
            stats.transformNormal = Vector3.up;

            _timer = 0f;
            
            var doc = SonicGameDocument.GetDocument("Sonic");
            var param = doc.GetGroup(SonicGameDocument.PhysicsGroup);
            actor.model.SetCollisionParam(param.GetParameter<float>("JumpCollisionHeight"), param.GetParameter<float>("JumpCollisionCenter"), param.GetParameter<float>("JumpCollisionRadius"));

            Common.ResetVelocity(ResetVelocityType.Both);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            animation.ResetAction();
            actor.model.SetCollisionParam(0, 0);
            _target = null;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            var param = SonicGameDocument.GetDocument("Sonic").GetGroup(SonicGameDocument.HomingGroup);
            
            if (_target != null)
            {
                Vector3 direction = (_target.position - actor.transform.position).normalized;
                _rigidbody.linearVelocity = direction * param.GetParameter<float>("HomingSpeed");
                _rigidbody.rotation = Quaternion.LookRotation(direction, Vector3.up);
                
                // If for some reason Sonic get stuck
                _timer += dt / param.GetParameter<float>("MaxTargetHomingTime");
                if (_timer >= 1f)
                {
                    stateMachine.SetState<FStateAir>();
                }
            }
            else
            {
                if (Common.CheckForGround(out _, CheckGroundType.PredictJump))
                {
                    stateMachine.SetState<FStateAir>();
                }
                
                Vector3 direction = actor.transform.forward;
                _rigidbody.linearVelocity = direction * (param.GetParameter<float>("HomingDistance") *
                                                         param.GetParameter<AnimationCurve>("HomingCurve").Evaluate(_timer));
                _rigidbody.rotation = Quaternion.LookRotation(direction, Vector3.up);
                
                _timer += dt / param.GetParameter<float>("HomingTime");
                if (_timer >= 1f)
                {
                    stateMachine.SetState<FStateAir>();
                }
            }
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }
    }
}