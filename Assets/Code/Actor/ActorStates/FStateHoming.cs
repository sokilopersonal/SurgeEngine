using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateHoming : FStateMove
    {
        private Transform _target;
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            animation.TransitionToState("Homing", 0f, true);

            Common.ResetVelocity();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            animation.ResetAction();
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            Vector3 direction = (_target.position - actor.transform.position).normalized;
            _rigidbody.linearVelocity = direction * actor.stats.homingParameters.homingSpeed;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }
    }
}