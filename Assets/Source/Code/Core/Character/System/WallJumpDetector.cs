using System;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class WallJumpDetector : MonoBehaviour
    {
        private const string WallJumpTag = "Surge Engine/WallJump";
        private const float WallNormalMultiplier = 0.4f;

        [Inject] private CharacterBase _character;
        private IWallJumpDetect _currentDetect;

        private void OnEnable()
        {
            _character.StateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnDisable()
        {
            _character.StateMachine.OnStateAssign -= OnStateAssign;
        }

        private void FixedUpdate()
        {
            if (_currentDetect != null)
                DetectWall();
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is IWallJumpDetect detect)
                _currentDetect = detect;
            else
                _currentDetect = null;
        }

        private void DetectWall() 
        {
            var body = _character.Rigidbody;
            var wallRay = new Ray(body.position, body.linearVelocity);
            if (Physics.Raycast(wallRay, out var wallHit, 0.5f, _character.Config.castLayer))
            {
                bool isWallJump = wallHit.collider.CompareTag(WallJumpTag) || _character.Kinematics.Path2D != null;
                if (isWallJump && Mathf.Abs(Vector3.Angle(wallHit.normal, Vector3.up) - 90f) < 0.02)
                {
                    _currentDetect.WallDetected = true;
                    body.position = wallHit.point + wallHit.normal * WallNormalMultiplier;
                    body.rotation = Quaternion.LookRotation(wallHit.normal) * Quaternion.Euler(0, 90, 0);
                    _character.StateMachine.SetState<FStateWall>();
                }
                else
                {
                    _currentDetect.WallDetected = false;
                }
            }
            else
            {
                _currentDetect.WallDetected = false;
            }
        }
    }
}