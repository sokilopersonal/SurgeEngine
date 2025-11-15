using SurgeEngine.Source.Code.Core.StateMachine;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    /// <summary>
    /// Represents a base class for defining various action sets for an actor in the system.
    /// </summary>
    public abstract class CharacterActions
    {
        protected readonly CharacterBase Character;
        
        protected FStateMachine StateMachine => Character.StateMachine;
        protected Rigidbody Rigidbody => Character.Rigidbody;
        
        protected CharacterKinematics Kinematics => Character.Kinematics;
        protected CharacterInput Input => Character.Input;
        protected CharacterModel Model => Character.Model;
        protected CharacterAnimation Animation => Character.Animation;
        protected CharacterFlags Flags => Character.Flags;

        protected CharacterActions(CharacterBase character)
        {
            Character = character;
            
            Connect(StateMachine);
        }

        public virtual void Execute() { }

        public virtual void FixedExecute() { }

        protected abstract void Connect(FStateMachine stateMachine);
    }
}