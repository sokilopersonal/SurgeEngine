using System;
using Cysharp.Threading.Tasks;
using SurgeEngine.Code.Custom;
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

            Common.ResetVelocity(ResetVelocityType.Both);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            animation.ResetAction();
            _target = null;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (_target != null)
            {
                Vector3 direction = (_target.position - actor.transform.position).normalized;
                _rigidbody.linearVelocity = direction * stats.homingParameters.homingSpeed;
                _rigidbody.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            else
            {
                if (Common.CheckForGround(out _, CheckGroundType.PredictJump))
                {
                    stateMachine.SetState<FStateAir>();
                }
                
                Vector3 direction = actor.transform.forward;
                _rigidbody.linearVelocity = direction * (stats.homingParameters.homingDistance *
                                                         stats.homingParameters.homingCurve.Evaluate(_timer));
                _rigidbody.rotation = Quaternion.LookRotation(direction, Vector3.up);
                
                _timer += dt / actor.stats.homingParameters.homingTime;
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