using System;
using System.Collections;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.StateMachine.Components
{
    public class StateAnimator : MonoBehaviour
    {
        public Animator animator;
        [SerializeField] private bool allowIK;
        [SerializeField] private string[] ikAllowedAnimations;

        private string _currentAnimation;
        public Coroutine AnimationDelayedCoroutine;
        private Coroutine _animationWaitCoroutine;
        private AnimationHandle _currentHandle;
        
        private Coroutine _coroutine;
        
        private void Update()
        {
            
        }

        public void StopWork()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            
            if (AnimationDelayedCoroutine != null)
                StopCoroutine(AnimationDelayedCoroutine);
        }

        public AnimationHandle TransitionToState(string stateName, float transitionTime = 0.25f)
        {
            if (_animationWaitCoroutine != null)
            {
                _currentHandle?.Cancel();
                StopCoroutine(_animationWaitCoroutine);
                _animationWaitCoroutine = null;
            }
            animator.TransitionToState(stateName, ref _currentAnimation, transitionTime);
    
            var transition = new AnimationHandle(this);
            _currentHandle = transition;
            _animationWaitCoroutine = StartCoroutine(WaitForAnimationEnd(stateName, transition));
            return transition;
        }

        public AnimationHandle TransitionToStateDelayed(string stateName, float delay, float transitionTime = 0.25f)
        {
            if (AnimationDelayedCoroutine != null)
            {
                StopCoroutine(AnimationDelayedCoroutine);
                AnimationDelayedCoroutine = null;
            }

            var handle = new AnimationHandle(this);
            AnimationDelayedCoroutine = StartCoroutine(DelayedTransitionCoroutine(stateName, delay, transitionTime, handle));
            return handle;
        }

        private IEnumerator DelayedTransitionCoroutine(string stateName, float delay, float transitionTime, AnimationHandle handle)
        {
            yield return new WaitForSeconds(delay);
            AnimationDelayedCoroutine = null;

            if (handle.IsCanceled)
                yield break;

            var animationHandle = TransitionToState(stateName, transitionTime);
            handle.LinkAnimationHandle(animationHandle);
            animationHandle.Then(() => handle.InvokeEnd());
        }

        private IEnumerator WaitForAnimationEnd(string stateName, AnimationHandle handle)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            while (!stateInfo.IsName(stateName))
            {
                yield return null;
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }
            while (stateInfo.normalizedTime < 1f)
            {
                yield return null;
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            }
            if (!handle.IsCanceled)
                handle.InvokeEnd();
            _animationWaitCoroutine = null;
            _currentHandle = null;
        }
        
        public void SetCurrentAnimationState(string state) => _currentAnimation = state;
        public string GetCurrentAnimationState() => _currentAnimation;
        public void ResetCurrentAnimationState() => _currentAnimation = string.Empty;

        public bool IsIKAllowed()
        {
            foreach (var state in ikAllowedAnimations)
            {
                if (state == _currentAnimation)
                    return true;
            }
            return false;
        }
    }
    
    public class AnimationHandle
    {
        private readonly StateAnimator _owner;
        private AnimationHandle _linkedAnimationHandle;

        public AnimationHandle(StateAnimator owner) => _owner = owner;

        public event Action OnAnimationEnd;
        public bool IsCanceled { get; private set; }

        internal void LinkAnimationHandle(AnimationHandle animationHandle)
        {
            _linkedAnimationHandle = animationHandle;
        }

        public void Cancel()
        {
            IsCanceled = true;
            if (_owner.AnimationDelayedCoroutine != null)
            {
                _owner.StopCoroutine(_owner.AnimationDelayedCoroutine);
                _owner.AnimationDelayedCoroutine = null;
            }
            _linkedAnimationHandle?.Cancel();
        }

        internal void InvokeEnd() => OnAnimationEnd?.Invoke();

        public void Then(Action action) => OnAnimationEnd += action;

        public void After(float delay, Action action)
        {
            _owner.AnimationDelayedCoroutine = _owner.StartCoroutine(DelayedAction(delay, action));
        }

        public void AfterThen(float delay, Action action)
        {
            OnAnimationEnd += () => _owner.AnimationDelayedCoroutine = _owner.StartCoroutine(DelayedAction(delay, action));
        }

        private IEnumerator DelayedAction(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}