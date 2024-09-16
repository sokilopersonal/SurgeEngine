using SurgeEngine.Code.ActorStates;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorAnimation : ActorComponent
    {
        public Animator animator;

        private void Update()
        {
            SetBool("Idle", actor.stateMachine.currentStateName == "FStateIdle");
            SetFloat("GroundSpeed", actor.stats.planarVelocity.magnitude);
            SetFloat("TurnAngle", actor.stats.GetSignedAngle() / 45f);
        }
        
        public void SetFloat(string name, float value)
        {
            animator.SetFloat(name, value);
        }
        
        public void SetBool(string name, bool value)
        {
            animator.SetBool(name, value);
        }
    }
}