using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Custom.Extensions
{
    public static class AnimatorExtensions
    {
        public static void TransitionToState(this Animator animator, string stateName, ref string currentAnimation, float transitionTime = 0.25f)
        {
            if (currentAnimation != stateName)
            {
                animator.CrossFadeInFixedTime(stateName, transitionTime, 0);
            }
            
            currentAnimation = stateName;
        }
    }
}