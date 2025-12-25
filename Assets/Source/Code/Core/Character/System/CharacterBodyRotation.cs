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
            
            Vector3 currentVelocity = Vector3.ProjectOnPlane(vector, normal);
            float currentSpeed = currentVelocity.magnitude;

            if (HasValidInput(inputDir))
            {
                RotateWithInput(inputDir, currentVelocity, currentSpeed, normal, angleDelta, rb);
            }
            else
            {
                RotateWithVelocity(currentVelocity, currentSpeed, normal, rb);
            }
            
            AlignToNormal(normal, rb);
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
            return inputDir.sqrMagnitude > 0.02f;
        }

        private void RotateWithInput(Vector3 inputDir, Vector3 currentVelocity, float currentSpeed, 
            Vector3 normal, float angleDelta, Rigidbody rb)
        {
            Vector3 targetDir = CalculateTargetDirection(inputDir, currentVelocity, currentSpeed, normal);
            float rotSpeed = CalculateRotationSpeed(angleDelta, currentSpeed);

            if (targetDir != Vector3.zero)
            {
                var targetRot = Quaternion.LookRotation(targetDir, normal);
                var towards = Quaternion.RotateTowards(rb.rotation, targetRot, rotSpeed * Time.fixedDeltaTime);
                var finalRot = Quaternion.Slerp(rb.rotation, towards, 128 * Time.fixedDeltaTime);
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
                float t = CalculateVelocityBlendFactor(currentSpeed);
                targetDir = Vector3.Slerp(inputDir.normalized, velDir, t * 10f).normalized;
            }

            return targetDir;
        }

        private float CalculateVelocityBlendFactor(float currentSpeed)
        {
            float speedRange = _character.Config.topSpeed - SpeedThreshold;
            float t = Mathf.Clamp01((currentSpeed - SpeedThreshold) / speedRange);
            return Mathf.Sqrt(t);
        }

        private float CalculateRotationSpeed(float angleDelta, float currentSpeed)
        {
            if (currentSpeed <= SpeedThreshold)
                return angleDelta;

            float speedRange = _character.Config.topSpeed - SpeedThreshold;
            float speedFactor = (currentSpeed - SpeedThreshold) / speedRange;
            float rotationMultiplier = Mathf.Lerp(1f, 0.15f, Mathf.Pow(speedFactor, 0.5f));
            
            return angleDelta * rotationMultiplier;
        }

        private void RotateWithVelocity(Vector3 currentVelocity, float currentSpeed, 
            Vector3 normal, Rigidbody rb)
        {
            if (currentSpeed > 0.1f)
            {
                Vector3 velocityDir = currentVelocity.normalized;
                Quaternion targetRotation = Quaternion.LookRotation(velocityDir, normal);
                var towards = Quaternion.RotateTowards(rb.rotation, targetRotation,
                    (128f + currentSpeed) * Time.fixedDeltaTime);
                rb.MoveRotation(towards);
            }
        }

        private void AlignToNormal(Vector3 normal, Rigidbody rb)
        {
            Quaternion upRotation = Quaternion.FromToRotation(rb.transform.up, normal) * rb.rotation;
            rb.MoveRotation(upRotation);
        }

        private void AlignToUpward()
        {
            var rb = _character.Kinematics.Rigidbody;
            Quaternion upAlignment = Quaternion.FromToRotation(_character.transform.up, Vector3.up) * rb.rotation;
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
    }
}