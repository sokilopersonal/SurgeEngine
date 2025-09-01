using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
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