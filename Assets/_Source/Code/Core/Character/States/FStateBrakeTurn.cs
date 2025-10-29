using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
{
    public class FStateBrakeTurn : FCharacterState, IDamageableState
    {
        private float _timer;

        private Quaternion _startRotation;
        private Quaternion _endRotation;
        
        public FStateBrakeTurn(CharacterBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0f;
            
            _startRotation = Rigidbody.rotation;
            _endRotation = Rigidbody.rotation * Quaternion.Euler(0f, 180f, 0f);
            Rigidbody.rotation =
                Quaternion.LookRotation(Vector3.Cross(Rigidbody.transform.right, Vector3.up), Vector3.up);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Kinematics.CheckForGround(out var result))
            {
                Kinematics.Point = result.point;
                Kinematics.Normal = Vector3.up;
                
                Kinematics.Snap(result.point, Kinematics.Normal);
                
                float duration = 0.45f;
                if (_timer < duration)
                {
                    Rigidbody.rotation = Quaternion.Lerp(_startRotation, _endRotation, Easings.Get(Easing.InSine, _timer / duration));
                    Model.Root.rotation = Rigidbody.rotation;
                
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