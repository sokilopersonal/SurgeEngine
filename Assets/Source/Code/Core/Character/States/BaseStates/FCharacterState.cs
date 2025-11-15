using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States.BaseStates
{
    public abstract class FCharacterState : FState
    {
        protected CharacterBase Character { get; }
        protected CharacterKinematics Kinematics => Character.Kinematics;
        protected CharacterInput Input => Character.Input;
        protected CharacterModel Model => Character.Model;
        protected CharacterAnimation Animation => Character.Animation;
        protected CharacterActions Actions { get; private set; }
        protected Rigidbody Rigidbody => Character.Rigidbody;
        protected FStateMachine StateMachine { get; private set; }

        protected FCharacterState(CharacterBase owner)
        {
            Character = owner;
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