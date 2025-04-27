using System;
using System.Collections;
using UnityEngine;

namespace SurgeEngine.Code.Core.StateMachine.Components
{
    public class StateAnimator : MonoBehaviour
    {
        private Animator _animator;
        public Animator Animator => _animator;

        private AnimationHandle _currentHandle;
        private string _currentAnimation;
        private bool _isWaiting;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (_currentHandle != null)
            {
                if (_isWaiting)
                {
                    if (_currentHandle.WaitForAnimationEnd(_currentAnimation))
                    {
                        _isWaiting = false;
                        _currentHandle.OnAnimationFinish?.Invoke();
                    }
                }
            }
        }

        public AnimationHandle TransitionToState(string state, float time = 0.25f)
        {
            _isWaiting = true;
            _currentHandle = new AnimationHandle(this); 
            if (_currentAnimation != state) _animator.CrossFadeInFixedTime(state, time, 0);
            _currentAnimation = state;
            return _currentHandle;
        }

        public AnimationHandle TransitionToStateDelayed(string state, float delay, float time = 0.25f)
        {
            StartCoroutine(TransitionToStateDelayedCoroutine(state, delay, time));
            return _currentHandle;
        }

        private IEnumerator TransitionToStateDelayedCoroutine(string state, float delay, float time = 0.25f)
        {
            yield return new WaitForSeconds(delay);
            TransitionToState(state, time);
        }
        
        public void SetCurrentAnimationState(string state) => _currentAnimation = state;
        public string GetCurrentAnimationState() => _currentAnimation;
        public AnimationHandle GetCurrentAnimationHandle() => _currentHandle;
        public void ResetCurrentAnimationState()
        {
            _isWaiting = false;
            _currentAnimation = string.Empty;
        }
    }

    public class AnimationHandle
    {
        private readonly StateAnimator _stateAnimator;
        public Action OnAnimationFinish;
        
        public AnimationHandle(StateAnimator stateAnimator)
        {
            _stateAnimator = stateAnimator;
            OnAnimationFinish = null;
        }

        public bool WaitForAnimationEnd(string state)
        {
            var stateInfo = _stateAnimator.Animator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName(state))
            {
                return false;
            }
            if (stateInfo.normalizedTime < 1f)
            {
                return false;
            }
            
            return true;
        }

        public void Then(Action callback) => OnAnimationFinish += callback;
        public void After(float time, Action callback) 
            => _stateAnimator.StartCoroutine(WaitCallback(time, callback));

        public void AfterThen(float time, Action callback)
        {
            OnAnimationFinish += () => _stateAnimator.StartCoroutine(WaitCallback(time, callback));
        }

        private IEnumerator WaitCallback(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }
    }
}