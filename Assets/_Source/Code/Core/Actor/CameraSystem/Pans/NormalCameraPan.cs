using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class NormalCameraPan : NewModernState, IPanState<NormalPanData>
    {
        private NormalPanData _nData;
        
        public NormalCameraPan(ActorBase owner) : base(owner)
        {
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            StateFOV = _nData.fov;
        }

        public void SetData(NormalPanData data)
        {
            _nData = data;
            _stateMachine.CurrentData = data;
        }

        protected override float GetDistance() => _nData.distance;
        protected override float GetVerticalOffset() => _nData.yOffset;

        protected override float CalculateCollisionDistance(Vector3 origin, Vector3 direction, float baseDistance)
        {
            return _nData.isCollision ? base.CalculateCollisionDistance(origin, direction, baseDistance) : baseDistance;
        }
    }
}