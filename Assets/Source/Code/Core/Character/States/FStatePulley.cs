using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStatePulley : FCharacterState
    {
        public FStatePulley(CharacterBase owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.interpolation = RigidbodyInterpolation.None;
            Kinematics.IsKinematic = true;
        }

        public override void OnExit()
        {
            base.OnExit();

            Kinematics.IsKinematic = false;
            Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }
}