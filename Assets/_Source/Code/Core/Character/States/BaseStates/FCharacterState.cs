using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Core.StateMachine;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States.BaseStates
{
    public abstract class FCharacterState : FState
    {
        protected CharacterBase character { get; }
        protected CharacterKinematics Kinematics => character.Kinematics;
        protected CharacterInput Input => character.Input;
        protected CharacterModel Model => character.Model;
        protected CharacterAnimation Animation => character.Animation;
        protected CharacterActions Actions { get; private set; }
        
        protected Rigidbody Rigidbody => character.Rigidbody;
        protected FStateMachine StateMachine { get; private set; }

        protected FCharacterState(CharacterBase owner)
        {
            character = owner;
            StateMachine = owner.StateMachine;
        }

        public override void OnTick(float dt)
        {
            Actions?.Execute();
            
            base.OnTick(dt);
        }

        public override void OnFixedTick(float dt)
        {
            Actions?.FixedExecute();
            
            base.OnFixedTick(dt);
        }

        public void SetActions(CharacterActions actions) => Actions = actions;
    }
}