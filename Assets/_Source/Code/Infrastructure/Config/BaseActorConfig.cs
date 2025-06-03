using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Config
{
    [CreateAssetMenu(fileName = "BaseActorConfig", menuName = "SurgeEngine/Configs/Physics/Base", order = 0)]
    public class BaseActorConfig : ScriptableObject
    {
        [Header("Cast")]
        public float castDistance = 1.15f;
        public AnimationCurve castDistanceCurve;
        public LayerMask castLayer;

        [Header("Base Physics")]
        public float topSpeed = 28;
        public float maxSpeed = 35;
        public float minParaloopSpeed = 28;
        
        [Header("Air Physics")]
        public float maxVerticalSpeed;
        [Range(0.1f, 1)] public float airControl = 0.4f;
        
        [Header("Acceleration")]
        public float accelerationRate = 7.5f;
        public AnimationCurve accelerationCurve;
        
        [Header("Deceleration")]
        public float minDecelerationRate = 18;
        public float maxDecelerationRate = 15;
        public float airDecelerationRate = 6;
        
        [Header("Skidding")]
        public float minSkiddingRate = 10;
        public float maxSkiddingRate = 15;
        public float skiddingThreshold = 0.5f;
        public float skiddingSpeedThreshold = 15f;
        
        [Header("Turn")]
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
        public float jumpMaxAirTime = 0.65f;

        [Header("Slope")]
        public float slopeDeslopeForce = 10;
        public float slopeDeslopeAngle = 30;
        public float slopeMinForceSpeed = 6;
        public float slopeInactiveDuration = 0.5f;
        public float slopeUphillForce = 10;
        public float slopeDownhillForce = 5;
        public float slopeMinAngle = 20;
        public float slopeMinSpeed = 6;
        
        [Header("Rails")]
        public LayerMask railMask;
        public float railSearchDistance = 10f;
        public AnimationCurve railSwitchCurve;
    }
}
