using System;
using SurgeEngine.Code.StateMachine.Components;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

namespace SurgeEngine.Code.Actor.System.IK
{
    public class ActorIKController : MonoBehaviour
    {
        [Header("Data")] 
        [SerializeField] private IKData leftFoot;
        [SerializeField] private IKData rightFoot;
        
        [Header("IK")]
        [SerializeField] private float weightSpeed = 14;
        
        [Header("Ray Settings")]
        [SerializeField] private float rayLength = 0.7f;
        [SerializeField] private float rayVerticalOffset = 0.1f;
        [SerializeField] private float plantedOffset = 0.1f;
        [SerializeField] private float hintVerticalOffset = 0.1f; 
        [SerializeField] private LayerMask cullingMask;
        
        private StateAnimator _stateAnimator;
        private float _ikWeight;

        private void Start()
        {
            _stateAnimator = GetComponent<StateAnimator>();

            _ikWeight = 1;

            leftFoot.Start = leftFoot.target.localPosition;
            rightFoot.Start = rightFoot.target.localPosition;
        }

        private void Update()
        {
            float value = _stateAnimator.IsIKAllowed() ? 1 : 0;
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
                var tarRot = Quaternion.FromToRotation(Vector3.up, hit.normal) * data.constraint.transform.rotation;
                data.target.rotation = tarRot;
                var pos = hit.point;
                pos.y += plantedOffset;
                data.target.position = pos;
                
                var hintPos = hit.point;
                hintPos.x = data.hint.localPosition.x;
                hintPos.y += hintVerticalOffset;
                hintPos.z = data.hint.localPosition.z;
                data.hint.localPosition = hintPos;
                
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
    }

    [Serializable]
    public struct IKData
    {
        public MultiParentConstraint constraint;
        public TwoBoneIKConstraint ik;
        public Transform target;
        public Transform hint;
        
        public Vector3 Origin { get; set; }
        public Vector3 Start { get; set; }
    }
}