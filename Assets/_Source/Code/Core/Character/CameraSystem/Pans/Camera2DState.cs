using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.CameraSystem.Pans
{
    public class Camera2DState : NewModernState
    {
        private Vector3 _right;
        private Vector3 _rightVelocity;
        private Vector3 _tg;
        private Vector3 _tgVelocity;
        
        public Camera2DState(CharacterBase owner) : base(owner)
        {
        }

        protected override void SetPosition(Vector3 targetPosition)
        {
            StatePosition = Calculate2DCameraTarget();
        }
        
        protected override void SetRotation(Vector3 actorPosition)
        {
            base.SetRotation(actorPosition + _tg);
        }
        
        private Vector3 Calculate2DCameraTarget()
        {
            var path2D = Character.Kinematics.Path2D;
            if (path2D == null)
            {
                return Vector3.zero;
            }

            path2D.Spline.EvaluateWorld(out var pos, out var tg, out var up, out var right);
            _right = Vector3.SmoothDamp(_right, right, ref _rightVelocity, 0.2f);
            
            tg *= Mathf.Sign(Vector3.Dot(Character.transform.forward, tg));
            _tg = Vector3.SmoothDamp(_tg, tg * 0.5f, ref _tgVelocity, 0.2f);

            return CalculateTarget() + _tg + _right * 6f;
        }
    }
}