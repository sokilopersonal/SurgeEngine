using System;
using SurgeEngine._Source.Code.Core.StateMachine.Components;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace SurgeEngine._Source.Code.Core.Character.System.IK
{
    public class CharacterIKController : MonoBehaviour
    {
        [Header("Data")] 
        [SerializeField] private IKData leftFoot;
        [SerializeField] private IKData rightFoot;
        
        [Header("IK")]
        [SerializeField] private float weightSpeed = 14;
        [SerializeField] private string[] allowedStates;
        
        [Header("Ray Settings")]
        [SerializeField] private float rayLength = 0.7f;
        [SerializeField] private float rayVerticalOffset = 0.1f;
        [SerializeField] private float plantedOffset = 0.1f;
        [SerializeField] private LayerMask cullingMask;
        
        private StateAnimator _stateAnimator;
        private float _ikWeight;

        private void Awake()
        {
            _stateAnimator = GetComponent<StateAnimator>();
            _ikWeight = 1;

            leftFoot.Start = leftFoot.target.localPosition;
            rightFoot.Start = rightFoot.target.localPosition;
            leftFoot.ik.weight = 0;
            rightFoot.ik.weight = 0;
            leftFoot.constraint.weight = 1;
            rightFoot.constraint.weight = 1;
            
            leftFoot.Origin = leftFoot.constraint.transform.position + Vector3.up * rayVerticalOffset;
            rightFoot.Origin = rightFoot.constraint.transform.position + Vector3.up * rayVerticalOffset;
        }

        private void LateUpdate()
        {
            float value = IsAllowed() ? 1 : 0;
            _ikWeight = Mathf.Lerp(_ikWeight, value, Time.deltaTime * weightSpeed);
            
            SolveIK(leftFoot);
            SolveIK(rightFoot);
        }

        private void SolveIK(IKData data)
        {
            data.ik.weight = 0;
            data.constraint.weight = 1;
            data.Origin = data.constraint.transform.position + Vector3.up * rayVerticalOffset;
            Color color = Color.red;
            
            if (Physics.Raycast(data.Origin, Vector3.down, out var hit, rayLength, cullingMask))
            {
                data.ik.weight = _ikWeight;
                data.target.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * data.constraint.transform.rotation;
                var pos = hit.point;
                pos.y += plantedOffset;
                data.target.position = pos;
                
                color = Color.green;
            }
            else
            {
                data.ik.weight = 0;
                data.target.localPosition = data.Start;
                data.target.localRotation = Quaternion.identity;
            }
            
            Debug.DrawRay(data.Origin, Vector3.down * rayLength, color);
        }

        private bool IsAllowed()
        {
            foreach (var allowedState in allowedStates)
            {
                if (_stateAnimator.GetCurrentAnimationState() == allowedState)
                {
                    return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    public struct IKData
    {
        public MultiParentConstraint constraint;
        public TwoBoneIKConstraint ik;
        public Transform target;
        
        public Vector3 Origin { get; set; }
        public Vector3 Start { get; set; }
    }
}