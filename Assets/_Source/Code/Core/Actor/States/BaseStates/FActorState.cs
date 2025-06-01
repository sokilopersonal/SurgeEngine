using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine;
using SurgeEngine.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.BaseStates
{
    public abstract class FActorState : FState
    {
        protected ActorBase Actor { get; }
        protected ActorKinematics Kinematics => Actor.Kinematics;
        protected ActorInput Input => Actor.Input;
        protected ActorModel Model => Actor.Model;
        protected ActorAnimation Animation => Actor.Animation;
        protected ActorActions Actions { get; private set; }
        
        protected Rigidbody Rigidbody => Actor.Rigidbody;
        protected FStateMachine StateMachine { get; private set; }

        protected FActorState(ActorBase owner)
        {
            Actor = owner;
            StateMachine = owner.StateMachine;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            Actions?.Execute();
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            Actions?.FixedExecute();
        }

        public void SetActions(ActorActions actions) => Actions = actions;
    }
}