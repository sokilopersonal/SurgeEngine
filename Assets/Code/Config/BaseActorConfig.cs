using UnityEngine;

namespace SurgeEngine.Code.Config
{
    [CreateAssetMenu(fileName = "BaseActorConfig", menuName = "SurgeEngine/Configs/Physics/Base", order = 0)]
    public class BaseActorConfig : ScriptableObject
    {
        [Header("Cast")]
        public float castDistance = 1.15f;
        public AnimationCurve castDistanceCurve;
        public LayerMask castLayer;
        public LayerMask railMask;

        [Header("Base Physics")]
        public float topSpeed = 28;
        public float maxSpeed = 35;
        public float minParaloopSpeed = 28;
        public float maxVerticalSpeed;
        public float accelerationRate = 7.5f;
        public AnimationCurve accelerationCurve;
        public float minDeaccelerationRate = 12;
        public float maxDeaccelerationRate = 20;
        public float minSkiddingRate = 10;
        public float maxSkiddingRate = 15;
        public float skiddingThreshold = 0.5f;
        public float skiddingSpeedThreshold = 15f;
        public float turnSpeed = 8;
        public AnimationCurve turnCurve;
        public float turnSmoothing = 10;

        [Header("Jump")]
        public float jumpForce = 16;
        public float jumpHoldForce = 12;
        public float jumpStartTime = 0.15f;
        public float jumpCollisionHeight = 0.6f;
        public float jumpCollisionCenter = 0.2f;
        public float jumpCollisionRadius = 0.5f;

        [Header("Slope")]
        public float slopeDeslopeForce = 10;
        public float slopeDeslopeAngle = 30;
        public float slopeMinForceSpeed = 6;
        public float slopeInactiveDuration = 0.5f;
        public float slopeUphillForce = 10;
        public float slopeDownhillForce = 5;
        public float slopeMinAngle = 20;
        public float slopeMinSpeed = 6;
    }
}
