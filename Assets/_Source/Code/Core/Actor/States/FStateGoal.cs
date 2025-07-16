using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.GoalRing;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateGoal : FActorState
    {
        public FStateGoal(ActorBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public void SetGoal(GoalRing goal)
        {
            Rigidbody.transform.position = goal.transform.position;
            Rigidbody.transform.rotation = Quaternion.Euler(0, goal.transform.rotation.eulerAngles.y, 0);

            if (Kinematics.CheckForGroundWithDirection(out var hit, Vector3.down, 5f))
            {
                Rigidbody.transform.position = hit.point + Vector3.up;
            }
            
            Rigidbody.isKinematic = true;
            
            const string ResultLook = "ResultLook";
            const string ReactionS = "ReactionS";
            const string ReactionA = "ReactionA";
            const string ReactionB = "ReactionB";
            const string ReactionC = "ReactionC";
            const string ReactionD = "ReactionD";
            const string ReactionE = "ReactionE";

            var animator = Animation.StateAnimator;
            animator.TransitionToStateDelayed(ResultLook, 1.5f)
                .Then(() => animator.TransitionToState(ReactionS));
        }
    }
}