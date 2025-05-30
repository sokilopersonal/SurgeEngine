using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateBrakeTurn : FStateMove, IDamageableState
    {
        private float _timer;

        private Quaternion _startRotation;
        private Quaternion _endRotation;
        
        public FStateBrakeTurn(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0f;
            
            _startRotation = _rigidbody.rotation;
            _endRotation = _rigidbody.rotation * Quaternion.Euler(0f, 180f, 0f);
            _rigidbody.rotation =
                Quaternion.LookRotation(Vector3.Cross(_rigidbody.transform.right, Vector3.up), Vector3.up);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Kinematics.CheckForGround(out var result))
            {
                Kinematics.Point = result.point;
                Kinematics.Normal = Vector3.up;
                
                Kinematics.Snap(result.point, Kinematics.Normal, true);
                
                float duration = 0.42f;
                if (_timer < duration)
                {
                    _rigidbody.rotation = Quaternion.Lerp(_startRotation, _endRotation, Easings.Get(Easing.InSine, _timer / duration));
                    Model.root.rotation = _rigidbody.rotation;
                
                    _timer += dt;
                }
                else
                {
                    StateMachine.SetState<FStateIdle>();
                }
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}