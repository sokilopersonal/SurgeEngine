using SurgeEngine.Code.Core.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
{
    public abstract class ActorActions : ActorComponent
    {
        protected FStateMachine StateMachine => Actor.StateMachine;
        protected Rigidbody Rigidbody => Actor.Rigidbody;
        
        protected ActorKinematics Kinematics => Actor.Kinematics;
        protected ActorInput Input => Actor.Input;
        protected ActorModel Model => Actor.Model;
        protected ActorAnimation Animation => Actor.Animation;
        protected ActorFlags Flags => Actor.Flags;

        private void Awake()
        {
            Connect(StateMachine);
        }

        public virtual void Execute() { }

        public virtual void FixedExecute() { }

        protected abstract void Connect(FStateMachine stateMachine);
    }
}