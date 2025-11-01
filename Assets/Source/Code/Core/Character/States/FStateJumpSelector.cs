using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateJumpSelector : FCharacterState
    {
        private JumpSelector _attachedJumpSelector;
        private float _timer;
        private float _fallTimer;
        private bool _willBeLaunched;
        
        public FStateJumpSelector(CharacterBase owner) : base(owner)
        {
            
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            _timer = 0;
            _fallTimer = 0;
            _willBeLaunched = false;
            
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.position = _attachedJumpSelector.transform.position + Vector3.up * 0.5f;
            Rigidbody.rotation = _attachedJumpSelector.transform.rotation;
            Model.Root.rotation = _attachedJumpSelector.transform.rotation;
            
            character.Flags.AddFlag(new Flag(FlagType.OutOfControl, false));
            
            _attachedJumpSelector.OnJumpSelectorResult?.Invoke(JumpSelectorResultType.Start);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            var inputTime = _attachedJumpSelector.InputTime;
            var launchTime = inputTime * 0.66f;
            var fallTime = inputTime + 1;
            
            bool aHeld = Input.AHeld;
            bool xHeld = Input.XHeld;
            bool bHeld = Input.BHeld;
            bool anyHeld = aHeld || xHeld || bHeld;

            if (anyHeld && _timer < inputTime)
            {
                _timer += dt;
                
                if (_timer >= launchTime && !_willBeLaunched)
                {
                    _willBeLaunched = true;
                    
                    _attachedJumpSelector.Rotate(aHeld ? JumpSelectorButton.A : bHeld ? JumpSelectorButton.B : JumpSelectorButton.X);
                }
            }

            if (!anyHeld && _fallTimer < fallTime)
            {
                _timer = 0;
                _fallTimer += dt;
            }

            if (_fallTimer >= fallTime)
            {
                _attachedJumpSelector.OnJumpSelectorResult?.Invoke(JumpSelectorResultType.Fall);
                character.Flags.RemoveFlag(FlagType.OutOfControl);
                StateMachine.SetState<FStateJumpSelectorLaunch>().SetData(0.5f, default, JumpSelectorResultType.Fall);
                return;
            }

            if (_timer >= inputTime)
            {
                if (aHeld)
                {
                    Launch(_attachedJumpSelector.UpShotForce, _attachedJumpSelector.UpShotOutOfControl, 
                        _attachedJumpSelector.transform.up, _attachedJumpSelector.transform.right, JumpSelectorButton.A, _attachedJumpSelector.UpShotPitch);
                }
                else if (xHeld)
                {
                    Launch(_attachedJumpSelector.ForwardShotForce, _attachedJumpSelector.ForwardShotOutOfControl,
                        _attachedJumpSelector.transform.forward, _attachedJumpSelector.transform.right, JumpSelectorButton.X);
                }
                else if (bHeld)
                {
                    Launch(_attachedJumpSelector.DownShotForce, _attachedJumpSelector.DownShotOutOfControl,
                        -_attachedJumpSelector.transform.up, _attachedJumpSelector.transform.right, JumpSelectorButton.B);
                }

                _attachedJumpSelector.OnJumpSelectorResult?.Invoke(JumpSelectorResultType.OK);
            }

            void Launch(float speed, float outOfControl, Vector3 forward, Vector3 right, JumpSelectorButton button, float pitch = 0)
            {
                Rigidbody.linearVelocity = Utility.GetImpulseWithPitch(forward, right, pitch, speed);
                character.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, outOfControl));
                
                StateMachine.SetState<FStateJumpSelectorLaunch>().SetData(outOfControl, button, JumpSelectorResultType.OK);
            }
        }

        public void AttachJumpSelector(JumpSelector jumpSelector)
        {
            _attachedJumpSelector = jumpSelector;
        }
    }
}