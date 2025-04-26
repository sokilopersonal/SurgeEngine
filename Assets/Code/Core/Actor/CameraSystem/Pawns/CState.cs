using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.StateMachine;

namespace SurgeEngine.Code.Actor.CameraSystem.Pawns
{
    public class CState : FState
    {
        protected readonly ActorBase _actor;
        protected readonly ActorCamera _master;
        protected readonly CameraStateMachine _stateMachine;

        protected object _data;

        public CState(ActorBase owner)
        {
            _actor = owner;

            _stateMachine = owner.camera.stateMachine;
            _master = _stateMachine.master;
        }
        
        public void SetData(object data)
        {
            _data = data;
        }
    }
}