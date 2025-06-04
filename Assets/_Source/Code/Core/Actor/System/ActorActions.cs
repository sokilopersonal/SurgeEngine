using SurgeEngine.Code.Core.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
{
    /// <summary>
    /// Represents a base class for defining various action sets for an actor in the system.
    /// </summary>
    public abstract class ActorActions
    {
        protected readonly ActorBase Actor;
        
        protected FStateMachine StateMachine => Actor.StateMachine;
        protected Rigidbody Rigidbody => Actor.Rigidbody;
        
        protected ActorKinematics Kinematics => Actor.Kinematics;
        protected ActorInput Input => Actor.Input;
        protected ActorModel Model => Actor.Model;
        protected ActorAnimation Animation => Actor.Animation;
        protected ActorFlags Flags => Actor.Flags;

        public ActorActions(ActorBase actor)
        {
            Actor = actor;
            
            Connect(StateMachine);
        }

        public virtual void Execute() { }

        public virtual void FixedExecute() { }

        protected abstract void Connect(FStateMachine stateMachine);
    }
}