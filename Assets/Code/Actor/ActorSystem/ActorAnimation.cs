using System;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorAnimation : ActorComponent
    {
        public Animator animator;

        public Transform model;

        private void Update()
        {
            SetBool(AnimatorParams.Idle, actor.stateMachine.currentStateName == "FStateIdle");
            SetBool(AnimatorParams.InAir, actor.stateMachine.currentStateName == "FStateAir" || actor.stateMachine.currentStateName == "FStateJump");
            SetFloat(AnimatorParams.GroundSpeed, Mathf.Clamp(actor.stats.planarVelocity.magnitude, 0, 25f));
            SetFloat(AnimatorParams.VerticalSpeed, actor.stats.currentVerticalSpeed);
            SetFloat(AnimatorParams.TurnAngle, Mathf.Lerp(animator.GetFloat("TurnAngle"), 
                -actor.transform.InverseTransformDirection(actor.stats.planarVelocity).x * 2f, 3.75f * Time.deltaTime));
            
            float dot = Vector3.Dot(Vector3.up, actor.transform.right);
            SetBool("OnWall", Mathf.Abs(dot) > 0.1f && actor.stats.groundAngle == 90);
            SetFloat("WallDot", -dot);
            SetFloat("AbsWallDot", Mathf.Abs(dot));

            model.localPosition = actor.transform.localPosition;
            model.localRotation = Quaternion.Slerp(model.localRotation, Quaternion.LookRotation(actor.transform.forward, actor.transform.up),
                12 * Time.deltaTime);
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
        
        public void TransitionToState(string stateName, float transitionTime = 0.25f, bool isAction = false)
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
        public static readonly int RunCycle = Animator.StringToHash("Run Cycle");
        public static readonly int AirCycle = Animator.StringToHash("Air Cycle");
    }
}