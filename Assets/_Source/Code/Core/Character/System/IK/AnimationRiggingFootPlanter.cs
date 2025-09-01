using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace SurgeEngine._Source.Code.Core.Character.System.IK
{
    public class AnimationRiggingFootPlanter : MonoBehaviour
    {
        [SerializeField] private MultiParentConstraint footRefConstraint;
        [SerializeField] private TwoBoneIKConstraint footIK;
        [SerializeField] private Transform IKTarget;
        [SerializeField] private float rayYOffset = 1;
        [SerializeField] private float rayDistance = 0.1f;
        [SerializeField] private float plantedYOffset = 0.1f;
        [SerializeField, Range(0, 0.5f)] private float positionTime = 0.15f;
        [SerializeField] private LayerMask mask;

        private Vector3 _rayOrigin;
        private Vector3 _positionVelocity;

        public void SolveIK(float multiplier)
        {
            footIK.weight = 0;
            footRefConstraint.weight = 1;
            transform.position = footRefConstraint.transform.position;
            _rayOrigin = transform.position + Vector3.up * rayYOffset;
            
            if (Physics.Raycast(_rayOrigin, Vector3.down, out var hit, rayDistance,mask))
            {
                footIK.weight = 1 * multiplier;
                var pos = hit.point;
                pos.y += plantedYOffset;
                IKTarget.position = Vector3.SmoothDamp(IKTarget.position, pos, ref _positionVelocity, positionTime);
                var tarRot = Quaternion.FromToRotation(Vector3.up, hit.normal) * footRefConstraint.transform.rotation;
                IKTarget.rotation = tarRot;
            }
            
            Debug.DrawRay(_rayOrigin, Vector3.down * rayDistance, Color.red);
        }
        
        public TwoBoneIKConstraint GetFootIKConstrain() => footIK;
    }
}