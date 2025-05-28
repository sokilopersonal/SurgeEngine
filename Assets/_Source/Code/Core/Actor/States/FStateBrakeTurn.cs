using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateBrakeTurn : FStateMove, IDamageableState
    {
        private float _timer;
        private Quaternion _rigidbodyRotation;
        
        public FStateBrakeTurn(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0f;
            
            _rigidbody.rotation = Quaternion.Euler(0f, _rigidbodyRotation.eulerAngles.y, 0f);
            _rigidbodyRotation = _rigidbody.rotation;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Kinematics.CheckForGround(out var result))
            {
                Kinematics.Point = result.point;
                Kinematics.Normal = Vector3.up;
                
                Kinematics.Snap(result.point, Kinematics.Normal, true);
                
                float duration = 0.3f;
                if (_timer < duration)
                {
                    _timer += dt;
                
                    _rigidbody.rotation *= Quaternion.AngleAxis(600f * dt, Vector3.up);
                    Model.root.rotation = _rigidbody.rotation;
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