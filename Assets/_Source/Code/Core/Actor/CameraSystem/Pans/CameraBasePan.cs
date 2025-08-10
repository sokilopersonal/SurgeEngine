using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class CameraBasePan<T> : CameraState, IPanState<T> where T : PanData
    {
        protected T _panData;
        protected CameraData Data;
        
        protected CameraBasePan(CharacterBase owner) : base(owner) { }

        public virtual void SetData(T data)
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