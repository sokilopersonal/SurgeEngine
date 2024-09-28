using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateHoming : FStateMove
    {
        private Transform _target;
        
        public override void OnEnter()
        {
            base.OnEnter();

            stateMachine.GetSubState<FBoost>().Active = false;
            animation.TransitionToState("Homing", 0f, true);

            Common.ResetVelocity(ResetVelocityType.Both);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            animation.ResetAction();
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (_target != null)
            {
                Vector3 direction = (_target.position - actor.transform.position).normalized;
                _rigidbody.linearVelocity = direction * actor.stats.homingParameters.homingSpeed;
                _rigidbody.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }
    }
}