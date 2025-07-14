using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class ParallelCameraPan : NewModernState, IPanState<ParallelPanData>
    {
        private LastCameraData _lastData;
        private ParallelPanData _vData;

        public ParallelCameraPan(ActorBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            _lastData = _stateMachine.RememberRelativeLastData();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            _stateMachine.SetDirection(_stateMachine.Transform.forward);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _stateMachine.SetDirection(_vData.forward);

            StateFOV = _vData.fov;
        }

        protected override void SetPosition(Vector3 targetPosition)
        {
            StatePosition = targetPosition;
        }
        
        protected override void SetRotation(Vector3 actorPosition)
        {
            StateRotation = Quaternion.LookRotation(actorPosition - StatePosition);
        }

        protected override void LookAxis()
        {
            
        }

        protected override void AutoLookDirection()
        {
            _stateMachine.YawAuto = 0;
            _stateMachine.PitchAuto = 0;
        }

        public void SetData(ParallelPanData data)
        {
            _vData = data;
            _stateMachine.CurrentData = data;
        }

        protected override float GetDistance() => _vData.distance;
    }
}