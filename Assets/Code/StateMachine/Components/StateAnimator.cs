using UnityEngine;

namespace SurgeEngine.Code.StateMachine.Components
{
    public abstract class StateAnimator : MonoBehaviour
    {
        public Animator animator;
        
        protected string _currentAnimation;

        private void Update()
        {
            AnimationTick();
        }

        protected abstract void AnimationTick();
        
        protected void SetFloat(string state, float value)
        {
            animator.SetFloat(state, value);
        }

        protected void SetBool(string state, bool value)
        {
            animator.SetBool(state, value);
        }

        protected void SetFloat(int id, float value)
        {
            animator.SetFloat(id, value);
        }

        protected void SetBool(int id, bool value)
        {
            animator.SetBool(id, value);
        }

        protected void SetAction(bool value)
        {
            SetBool("InAction", value);
        }

        public void ResetAction()
        {
            SetAction(false);
        }

        public void TransitionToState(string stateName, float transitionTime = 0.25f, bool isAction = false)
        {
            if (_currentAnimation != stateName)
            {
                animator.CrossFadeInFixedTime(stateName, transitionTime);
            }
            
            _currentAnimation = stateName;

            if (isAction) SetAction(true);
        }
    }
}