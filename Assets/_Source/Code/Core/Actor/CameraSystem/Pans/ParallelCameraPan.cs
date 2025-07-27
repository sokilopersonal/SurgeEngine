using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class ParallelCameraPan : NewModernState, IPanState<ParallelPanData>
    {
        private ParallelPanData _vData;

        public ParallelCameraPan(ActorBase owner) : base(owner)
        {
            
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

        protected override void YLag(float min, float max)
        {
            base.YLag(-0.4f, 0.4f);
        }

        protected override float CalculateCollisionDistance(Vector3 origin, Vector3 direction, float baseDistance)
        {
            return _vData.isCollision ? base.CalculateCollisionDistance(origin, direction, baseDistance) : baseDistance;
        }

        public void SetData(ParallelPanData data)
        {
            _vData = data;
            _stateMachine.CurrentData = data;
        }

        protected override float GetDistance() => _vData.distance;
        protected override float GetVerticalOffset() => _vData.yOffset;
    }
}