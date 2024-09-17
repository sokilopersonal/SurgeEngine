using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorAnimation : ActorComponent
    {
        public Animator animator;

        private void Update()
        {
            SetBool(AnimatorParams.Idle, actor.stateMachine.currentStateName == "FStateIdle");
            SetBool(AnimatorParams.InAir, actor.stateMachine.currentStateName == "FStateAir");
            SetFloat(AnimatorParams.GroundSpeed, actor.stats.planarVelocity.magnitude);
            SetFloat(AnimatorParams.VerticalSpeed, actor.stats.planarVelocity.y);
            SetFloat(AnimatorParams.TurnAngle, actor.transform.InverseTransformDirection(actor.stats.planarVelocity).x * 0.2f);
        }
        
        public void SetFloat(string state, float value)
        {
            animator.SetFloat(state, value);
        }
        
        public void SetBool(string state, bool value)
        {
            animator.SetBool(state, value);
        }
        
        public void SetFloat(int id, float value)
        {
            animator.SetFloat(id, value);
        }
        
        public void SetBool(int id, bool value)
        {
            animator.SetBool(id, value);
        }

        public void SetAction(bool value)
        {
            SetBool("InAction", value);
        }
        
        public void TransitionToState(string stateName, float transitionTime = 0.25f, bool isAction = true)
        {
            animator.CrossFade(stateName, transitionTime);
            
            if (isAction) SetAction(true);
        }
    }

    public static class AnimatorParams
    {
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int InAir = Animator.StringToHash("InAir");
        public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
        public static readonly int GroundSpeed = Animator.StringToHash("GroundSpeed");
        public static readonly int TurnAngle = Animator.StringToHash("TurnAngle");
        public static readonly string RunCycle = "Run Cycle"; 
    }
}