using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.GoalRing;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
{
    public class FStateGoal : FCharacterState
    {
        public FStateGoal(CharacterBase owner) : base(owner) { }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Animation.StateAnimator.TransitionToState("Idle");
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
        }

        public void LookAnimation(GoalRank rank)
        {
            const string ResultLook = "ResultLook";
            const string ReactionS = "ReactionS";
            const string ReactionA = "ReactionA";
            const string ReactionB = "ReactionB";
            const string ReactionC = "ReactionC";
            const string ReactionD = "ReactionD";
            const string ReactionE = "ReactionE";

            string reaction = ReactionE;

            switch (rank)
            {
                case GoalRank.S:
                    reaction = ReactionS;
                    break;
                case GoalRank.A:
                    reaction = ReactionA;
                    break;
                case GoalRank.B:
                    reaction = ReactionB;
                    break;
                case GoalRank.C:
                    reaction = ReactionC;
                    break;
                case GoalRank.D:
                    reaction = ReactionD;
                    break;
                case GoalRank.E:
                    reaction = ReactionE;
                    break;
            }

            var animator = Animation.StateAnimator;
            animator.TransitionToState(ResultLook)
                .Then(() => animator.TransitionToState(reaction));
        }
    }
}