using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateUpreel : FCharacterState
    {
        public FStateUpreel(CharacterBase owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.isKinematic = true;
        }

        public override void OnExit()
        {
            base.OnExit();

            Rigidbody.isKinematic = false;
        }
    }
}