using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.StateMachine;

namespace SurgeEngine.Code.Actor.States.BaseStates
{
    public abstract class FActorState : FState
    {
        protected ActorBase Actor { get; private set; }
        protected ActorInput Input { get; private set; }
        protected ActorStats Stats { get; private set; }
        protected ActorAnimation Animation { get; private set; }
        protected ActorKinematics Kinematics { get; private set; }
        protected ActorModel Model { get; private set; }
        protected FStateMachine StateMachine { get; private set; }

        protected FActorState(ActorBase owner)
        {
            Actor = owner;
            
            StateMachine = owner.stateMachine;
            
            Input = owner.input;
            Stats = owner.stats;
            Animation = owner.animation;
            Kinematics = owner.kinematics;
            Model = owner.model;
        }
    }
}