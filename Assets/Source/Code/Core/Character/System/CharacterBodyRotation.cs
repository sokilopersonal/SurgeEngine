using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    /// <summary>
    /// Handles character body rotation logic
    /// </summary>
    public class CharacterBodyRotation
    {
        private readonly CharacterBase _character;
        private const float SpeedThreshold = 3.5f;

        public CharacterBodyRotation(CharacterBase character)
        {
            _character = character;
        }

        /// <summary>
        /// Rotates body based on velocity direction
        /// </summary>
        public void RotateBody(Vector3 normal)
        {
            Vector3 vel = _character.Kinematics.Velocity;
            if (vel.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(vel, normal);
                _character.Kinematics.Rigidbody.MoveRotation(targetRotation);
            }
        }

        /// <summary>
        /// Rotates body with advanced input and velocity blending
        /// </summary>
        public void RotateBody(Vector3 vector, Vector3 normal, float angleDelta = 1200f)
        {
            var kinematics = _character.Kinematics;
            var rb = kinematics.Rigidbody;
            Vector3 inputDir = kinematics.GetInputDir();
            
            AlignToNormal(normal, rb);
            
            Vector3 currentVelocity = Vector3.ProjectOnPlane(vector, normal);
            float currentSpeed = currentVelocity.magnitude;
            RotateWithInput(HasValidInput(inputDir) ? inputDir : rb.transform.forward, currentVelocity, currentSpeed, normal, angleDelta, rb);
        }

        /// <summary>
        /// Rotates character to face velocity direction
        /// </summary>
        public void VelocityRotation(Vector3 vel)
        {
            float dot = Vector3.Dot(vel, Vector3.up);
            Vector3 left = Vector3.Cross(vel, Vector3.up);

            if (dot >= 0.99f)
            {
                AlignToUpward();
            }
            else
            {
                AlignToVelocity(vel, left);
            }
        }
        
        private bool HasValidInput(Vector3 inputDir)
        {
            return inputDir.magnitude > 0.02f;
        }

        private void RotateWithInput(Vector3 inputDir, Vector3 currentVelocity, float currentSpeed, 
            Vector3 normal, float angleDelta, Rigidbody rb)
        {
            Vector3 targetDir = CalculateTargetDirection(inputDir, currentVelocity, currentSpeed, normal);
            float rotSpeed = CalculateRotationSpeed(angleDelta, currentSpeed);

            if (targetDir != Vector3.zero)
            {
                var targetRot = Quaternion.LookRotation(targetDir, normal);
                var finalRot = Quaternion.RotateTowards(rb.rotation, targetRot, rotSpeed * Time.deltaTime);
                rb.MoveRotation(finalRot);
            }
        }

        private Vector3 CalculateTargetDirection(Vector3 inputDir, Vector3 currentVelocity, 
            float currentSpeed, Vector3 normal)
        {
            Vector3 targetDir = Vector3.ProjectOnPlane(inputDir.normalized, Vector3.up);

            if (currentSpeed > SpeedThreshold)
            {
                var velDir = Vector3.ProjectOnPlane(currentVelocity.normalized, normal);
                float t = CalculateVelocityBlendFactor(currentSpeed) * 8f;
                targetDir = Vector3.Slerp(inputDir.normalized, velDir, t).normalized;
            }

            return targetDir;
        }

        private float CalculateVelocityBlendFactor(float currentSpeed)
        {
            float speedRange = _character.Config.topSpeed - SpeedThreshold;
            float t = Mathf.Clamp01((currentSpeed - SpeedThreshold) / speedRange);
            return t;
        }

        private float CalculateRotationSpeed(float angleDelta, float currentSpeed)
        {
            float t = Mathf.InverseLerp(SpeedThreshold, _character.Config.topSpeed, currentSpeed);
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            float rotationMultiplier = Mathf.Lerp(1f, 0.5f, smooth);
            return angleDelta * rotationMultiplier;
        }

        private void AlignToNormal(Vector3 normal, Rigidbody rb)
        {
            Quaternion upRotation = Quaternion.FromToRotation(rb.transform.up, normal) * rb.rotation;
            rb.MoveRotation(upRotation);
        }

        private void AlignToUpward()
        {
            var rb = _character.Kinematics.Rigidbody;
            Quaternion upAlignment = Quaternion.FromToRotation(_character.Rigidbody.transform.up, Vector3.up) * rb.rotation;
            rb.MoveRotation(upAlignment);
        }

        private void AlignToVelocity(Vector3 vel, Vector3 left)
        {
            if (vel.sqrMagnitude > 0.1f)
            {
                Vector3 forward = Vector3.Cross(vel, left);
                _character.Kinematics.Rigidbody.MoveRotation(Quaternion.LookRotation(forward, vel));
            }
        }
        
        public bool AlignToUpOverTime(float deltaTime, ref float remainingTime)
        {
            var rb = _character.Kinematics.Rigidbody;

            Vector3 currentUp = rb.transform.up;
            float angle = Vector3.Angle(currentUp, Vector3.up);

            if (angle < 0.1f)
                return true;

            float t = 1f;
            if (remainingTime > 0f)
            {
                t = Mathf.Clamp01(deltaTime / remainingTime);
                remainingTime -= deltaTime;
            }

            Vector3 newUp = Vector3.Slerp(currentUp, Vector3.up, t).normalized;
            Vector3 forward = rb.transform.forward;
            Vector3 newForward = Vector3.ProjectOnPlane(forward, newUp).normalized;
            if (newForward.sqrMagnitude < 0.001f)
                newForward = Vector3.ProjectOnPlane(rb.transform.right, newUp).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(newForward, newUp);
            rb.MoveRotation(targetRotation);

            return remainingTime <= 0f;
        }
    }
}