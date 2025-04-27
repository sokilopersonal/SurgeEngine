using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pawns
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