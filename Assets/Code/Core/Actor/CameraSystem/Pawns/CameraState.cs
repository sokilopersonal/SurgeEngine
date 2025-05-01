using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns.Data;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pawns
{
    public class CameraState<T> : FState where T : PanData
    {
        protected readonly ActorBase _actor;
        protected readonly ActorCamera _master;
        protected readonly CameraStateMachine _stateMachine;

        protected T _panData;
        protected LastCameraData _lastData;

        public CameraState(ActorBase owner)
        {
            _actor = owner;

            _stateMachine = owner.camera.stateMachine;
            _master = _stateMachine.master;
        }

        public void SetData(PanData data)
        {
            _panData = data as T;
            _stateMachine.currentData = _panData;
        }
    }
}