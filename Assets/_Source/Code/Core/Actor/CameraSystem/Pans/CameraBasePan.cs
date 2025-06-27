using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class CameraBasePan<T> : CameraState, IPanState<T> where T : PanData
    {
        protected T _panData;
        protected LastCameraData _lastData;
        
        protected CameraBasePan(ActorBase owner) : base(owner) { }

        public void SetData(T data)
        {
            _panData = data;
            _stateMachine.CurrentData = data;
        }
    }
    
    public interface IPanState<in T> where T : PanData
    {
        void SetData(T data);
    }
}