using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class CameraState : FState
    {
        protected readonly ActorBase _actor;
        protected readonly ActorCamera _master;
        protected readonly CameraStateMachine _stateMachine;

        protected CameraState(ActorBase owner)
        {
            _actor = owner;
            _stateMachine = owner.Camera.StateMachine;
            _master= _stateMachine.Master;
        }
    }
}