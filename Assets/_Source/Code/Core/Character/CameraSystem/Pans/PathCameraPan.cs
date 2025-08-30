using SurgeEngine.Code.Core.Actor.CameraSystem.Pans.Data;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Pans
{
    public class PathCameraPan : CameraBasePan<PathPanData>
    {
        private float _timer;
        private SplineData _splineData;

        public PathCameraPan(CharacterBase owner) : base(owner) { }
        
        public override void OnExit()
        {
            base.OnExit();
            
            _stateMachine.SetDirection(_stateMachine.Transform.forward);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            _splineData.EvaluateWorld(out var pos, out var tg, out _, out _);
            
            Vector3 target = pos + tg * _panData.offsetOnEye;
            StatePosition = Vector3.Lerp(StatePosition, target, 32 * dt); // A little bit of smoothness
            StateRotation = SetRotation(Quaternion.LookRotation(tg));
            StateFOV = _panData.fov;
            
            _splineData.Time += Vector3.Dot(Character.Kinematics.Velocity, tg) * dt;
        }

        protected virtual Quaternion SetRotation(Quaternion rotation)
        {
            return rotation;
        }

        public override void SetData(PathPanData data)
        {
            base.SetData(data);
            
            _splineData = new SplineData(data.container, Character.Rigidbody.position);
        }
    }
}