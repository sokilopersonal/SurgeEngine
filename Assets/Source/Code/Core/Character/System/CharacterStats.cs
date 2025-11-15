using System;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class CharacterStats : CharacterComponent
    {
        public StompParameters stompParameters;
        public float turnRate;
        public float currentSpeed;
        public float currentVerticalSpeed;
        public Vector3 movementVector;
        public Vector3 planarVelocity;
        public Vector3 inputDir;
        public Vector3 groundNormal;
        public float moveDot;
        public bool skidding;
        public float groundAngle;
        public StageObject lastContactObject;
        public bool isGrounded;
        public bool isInAir;
        [HideInInspector] public float gravity;
        [field: SerializeField] public float startGravity { get; private set; }
        public HomingTarget homingTarget;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            startGravity = Mathf.Abs(Physics.gravity.y);
            gravity = startGravity;

            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            currentSpeed = _rigidbody.linearVelocity.magnitude;
            currentVerticalSpeed = _rigidbody.linearVelocity.y;
            
            moveDot = Vector3.Dot(Character.Kinematics.GetInputDir().normalized, _rigidbody.linearVelocity.normalized);

            FState state = Character.StateMachine.CurrentState;
            isGrounded = state is FStateGround;
            isInAir = state is FStateAir or FStateAirBoost or FStateJump or FStateSpecialJump;
            
            if (isInAir)
            {
                //homingTarget = SonicTools.FindHomingTarget();
            }
            else
            {
                homingTarget = null;
            }
        }

        public float GetForwardSignedAngle()
        {
            Vector3 forward = Character.transform.forward;
            Vector3 f = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;
            Vector3 c = Vector3.ProjectOnPlane(Character.Camera.GetCameraTransform().forward, Vector3.up).normalized;
            float dot = Vector3.Dot(Character.transform.up, Vector3.down);
            return f.SignedAngleByAxis(c, dot > 0 ? Vector3.up : Vector3.down);
        }

        public float GetUpwardSignedAngle()
        {
            Vector3 f = Vector3.ProjectOnPlane(Character.transform.up, Vector3.up).normalized;
            Vector3 c = Vector3.ProjectOnPlane(Character.Camera.GetCameraTransform().up, Vector3.up).normalized;
            return Vector3.SignedAngle(f, c, -Vector3.up);
        }

    }

    [Serializable]
    public class StompParameters
    {
        public float stompSpeed;
        public AnimationCurve stompCurve;
    }
}