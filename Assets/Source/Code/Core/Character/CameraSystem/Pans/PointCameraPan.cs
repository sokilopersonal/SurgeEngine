using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans.Data;
using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans
{
    public class PointCameraPan : NewModernState, IPanState<PointPanData>
    {
        private PointPanData _pData;

        public PointCameraPan(CharacterBase owner) : base(owner) { }
        
        public override void OnExit()
        {
            base.OnExit();
            
            _stateMachine.SetDirection(_stateMachine.Transform.transform.forward);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _stateMachine.SetDirection((_pData.target.position - StatePosition).normalized);

            StateFOV = _pData.fov;
        }
        
        protected override void SetPosition(Vector3 targetPosition)
        {
            StatePosition = targetPosition;
        }
        
        protected override void SetRotation(Vector3 actorPosition)
        {
            StateRotation = Quaternion.LookRotation(_pData.target.position - StatePosition);
        }

        protected override void LookAxis()
        {
            
        }

        public void SetData(PointPanData data)
        {
            _pData = data;
            _stateMachine.CurrentData = data;
        }

        protected override float CalculateCollisionDistance(Vector3 origin, Vector3 direction, float baseDistance)
        {
            return _pData.isCollision ? base.CalculateCollisionDistance(origin, direction, baseDistance) : baseDistance;
        }

        protected override float GetDistance() => _pData.distance;
        protected override float GetVerticalOffset() => _pData.yOffset;
    }
}