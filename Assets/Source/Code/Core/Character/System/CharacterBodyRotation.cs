using SurgeEngine.Source.Code.Infrastructure.Custom;
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
        private const float AngleThreshold = 0.1f;

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
                var towards = Quaternion.RotateTowards(rb.rotation, targetRot, rotSpeed * Time.fixedDeltaTime);
                var finalRot = Quaternion.Slerp(rb.rotation, towards, SurgeMath.Damp(0.4f, 64 * Time.fixedDeltaTime));
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
                targetDir = Vector3.Slerp(inputDir.normalized, velDir, t * 5f).normalized;
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
            float rotationMultiplier = Mathf.Lerp(1f, 0.5f, Mathf.Pow(speedFactor, 0.5f));
            
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
            
            float currentAngle = Vector3.Angle(rb.transform.up, Vector3.up);
            if (currentAngle < AngleThreshold)
            {
                return true;
            }

            if (remainingTime <= 0)
            {
                SmoothAlignToUp(100f);
                return true;
            }

            float rotationSpeed = currentAngle / remainingTime;

            Vector3 currentForward = rb.transform.forward;
            Vector3 right = Vector3.Cross(rb.transform.up, currentForward);
            Vector3 newForward = Vector3.Cross(right, Vector3.up).normalized;
            if (newForward == Vector3.zero)
            {
                newForward = currentForward;
            }
            
            Quaternion targetRotation = Quaternion.LookRotation(newForward, Vector3.up);

            float step = rotationSpeed * deltaTime;
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, step));

            remainingTime -= deltaTime;
            
            return false;
        }
        
        public void SmoothAlignToUp(float speed = 5f)
        {
            var rb = _character.Kinematics.Rigidbody;
            Quaternion target = Quaternion.FromToRotation(rb.transform.up, Vector3.up) * rb.rotation;
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, target, speed * Time.fixedDeltaTime));
        }
    }
}