using System;
using System.Collections;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.StateMachine.Components
{
    public abstract class StateAnimator : MonoBehaviour
    {
        public Animator animator;
        
        protected string _currentAnimation;
        public Coroutine animationDelayedCoroutine;
        private Coroutine _animationWaitCoroutine;
        private AnimationHandle _currentHandle;
        
        private Coroutine _coroutine;
        protected FStateMachine stateMachine;

        public void Initialize(FStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
            
            this.stateMachine.OnStateAssign += ChangeStateAnimation;
        }

        private void Update()
        {
            AnimationTick();
        }

        protected virtual void ChangeStateAnimation(FState obj)
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            
            if (animationDelayedCoroutine != null)
                StopCoroutine(animationDelayedCoroutine);
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
        
        protected abstract void AnimationTick();
        
        public void ResetCurrentAnimationState() => _currentAnimation = string.Empty;
        public string GetCurrentAnimationState() => _currentAnimation;
    }
    
    public class AnimationHandle
    {
        private StateAnimator _owner;
        public AnimationHandle(StateAnimator owner) => _owner = owner;

        public event Action OnAnimationEnd;
        public bool IsCanceled { get; private set; }
        public void Cancel()
        {
            IsCanceled = true;
        }
        internal void InvokeEnd() => OnAnimationEnd?.Invoke();

        /// <summary>
        /// Adds an action to be called when the animation ends
        /// </summary>
        /// <param name="action"></param>
        public void Then(Action action) => OnAnimationEnd += action;

        /// <summary>
        /// Adds an action to be called after a delay after the animation starts
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        public void After(float delay, Action action)
        {
            _owner.animationDelayedCoroutine = _owner.StartCoroutine(DelayedAction(delay, action));
        }

        /// <summary>
        /// Adds an action to be called after a delay after the animation ends
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        public void AfterThen(float delay, Action action)
        {
            OnAnimationEnd += () => _owner.animationDelayedCoroutine = _owner.StartCoroutine(DelayedAction(delay, action));
        }

        private IEnumerator DelayedAction(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}