using System;
using DG.Tweening;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom.Drawers;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    [SelectionBase]
    public class JumpSelector : ContactBase
    {
        [SerializeField] private float forwardForce = 25f;
        [SerializeField] private float upForce = 15f;
        [SerializeField] private float keepVelocityTime = 0.25f;
        [SerializeField] private float outOfControl = 1f;
        public JumpSelectorButton button;

        private JumpSelectorView _view;
        
        private float _timer;
        private float _holdTimer;
        private bool _playerInside;
        private bool _launching;

        private Vector3 _startPos;
        private Vector3 _direction;
        
        private ActorBase _actor;
        private Sequence _rotationSequence;

        public event Action<JumpSelectorResultType> OnJumpSelectorResult;

        protected override void Awake()
        {
            _view = GetComponentInChildren<JumpSelectorView>();
        }

        protected override void Update()
        {
            _actor ??= ActorContext.Context;
            
            if (_playerInside)
            {
                bool aPressed = _actor.input.JumpHeld;
                bool bPressed = _actor.input.BHeld;
                bool xPressed = _actor.input.BoostHeld;

                bool anyButton = aPressed
                    || bPressed
                    || xPressed;
                
                int index = (int)button;
                bool correct = aPressed && index == 0
                    || xPressed && index == 1
                    || bPressed && index == 2;
            
                if (_timer > 0)
                {
                    _timer -= Time.deltaTime;
                }
                else
                {
                    _actor.stateMachine.SetState<FStateAir>();
                    _actor.flags.RemoveFlag(FlagType.OutOfControl);
                    OnJumpSelectorResult?.Invoke(JumpSelectorResultType.Fall);
                    _playerInside = false;
                }

                if (anyButton || _launching)
                {
                    _holdTimer += Time.deltaTime;

                    if (_holdTimer >= 0.25f)
                    {
                        if (!_launching)
                        {
                            _direction = aPressed ? Vector3.up : bPressed ? Vector3.down : transform.forward;

                            if (_rotationSequence.IsActive())
                            {
                                _rotationSequence.Kill();
                            }

                            if (_direction == Vector3.up || _direction == Vector3.down)
                            {
                                _rotationSequence = DOTween.Sequence();
                                _rotationSequence.Append(_view.transform.DOLocalRotate(new Vector3(_direction == Vector3.up ? -90 : 90, 0, 0), 0.3f).SetEase(Ease.Linear));
                                _rotationSequence.Append(_view.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.Linear).SetDelay(0.4f));
                            }

                            _launching = true;
                        }
                    }

                    if (_holdTimer >= 0.7f)
                    {
                        Vector3 force = _direction * (_direction == Vector3.up ? upForce : _direction == transform.forward ? forwardForce : 0f);
                        _actor.stateMachine.SetState<FStateJumpSelectorLaunch>().SetData(force, keepVelocityTime);
                        
                        if (_direction == Vector3.up)
                        {
                            //_actor.animation.TransitionToState("SelectJumpS", 0f);
                        }
                        else
                        {
                            //_actor.animation.TransitionToState("Ball", 0f);
                        }
                        
                        if (correct)
                        {
                            OnJumpSelectorResult?.Invoke(JumpSelectorResultType.OK);
                        }
                        else
                        {
                            OnJumpSelectorResult?.Invoke(JumpSelectorResultType.Wrong);
                        }
                        
                        _actor.flags.AddFlag(new Flag(FlagType.OutOfControl, null, true, outOfControl));
                        _timer = 0;
                        _launching = false;
                        _playerInside = false;
                    }
                }
            }
        }

        public override void Contact(Collider msg)
        {
            _actor.stateMachine.SetState<FStateJumpSelector>();
            OnJumpSelectorResult?.Invoke(JumpSelectorResultType.Start);
            
            _startPos = _actor.transform.position;
            _timer = 2f;
            _holdTimer = 0;
            _playerInside = true;
            _launching = false;
            
            _actor.PutIn(transform.position + Vector3.up * 0.5f);
            
            Quaternion rot = Quaternion.LookRotation(transform.forward, Vector3.up);
            _actor.transform.rotation = rot;
            _actor.model.root.localRotation = rot;
            
            _actor.flags.AddFlag(new Flag(FlagType.OutOfControl, null, false));
        }

        private void OnDrawGizmosSelected()
        {
            TrajectoryDrawer.DrawTrajectory(transform.position, transform.forward, Color.blue, forwardForce, keepVelocityTime);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Vector3.up * upForce);
        }

        private void OnDestroy()
        {
            if (_rotationSequence.IsActive())
            {
                _rotationSequence.Kill();
            }
        }
    }

    public enum JumpSelectorResultType
    {
        Start,
        OK,
        Wrong,
        Fall
    }
    
    public enum JumpSelectorButton
    {
        A,
        X,
        B,
        U
    }
}