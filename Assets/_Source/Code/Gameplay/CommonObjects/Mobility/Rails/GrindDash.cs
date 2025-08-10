using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility.Rails
{
    public class GrindDash : ContactBase
    {
        [Header("Dash")]
        [SerializeField] private float speed = 25f;
        [SerializeField] private bool isForward = true;

        [Header("Spline")] 
        [SerializeField] private SplineContainer container;
        [SerializeField, Range(0, 1)] private float splineTime;
        [SerializeField, Range(0, 1f)] private float verticalOffset = 0.35f;

#if UNITY_EDITOR
        private bool _wereSplineAssigned;
#endif

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            if (context.StateMachine.CurrentState is FStateGrind grind)
            {
                context.Kinematics.Rigidbody.linearVelocity = transform.forward * speed;
                grind.SetForward(isForward);
            }
            else
            {
                Debug.LogWarning($"Object {gameObject.name} should be placed on rails.", this);
            }
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (!_wereSplineAssigned)
            {
                if (container != null)
                {
                    var newTime = PutInClosest();

                    if (Mathf.Abs(splineTime - newTime) > 0.01f)
                    {
                        splineTime = PutInClosest();
                    }
                    
                    _wereSplineAssigned = true;
                }
            }
            
            if (container != null)
            {
                container.Spline.Evaluate(splineTime, out var pos, out var tg, out var up);

                transform.position = container.transform.TransformPoint(pos + up * verticalOffset);
                transform.rotation = Quaternion.LookRotation(container.transform.TransformDirection(isForward ? tg : -tg), container.transform.TransformDirection(up));
            }

            if (_wereSplineAssigned)
            {
                if (container == null)
                {
                    splineTime = 0;
                    _wereSplineAssigned = false;
                }
            }
#endif
        }

        private float PutInClosest()
        {
            if (container != null)
            {
                SplineUtility.GetNearestPoint(container.Spline, container.transform.InverseTransformPoint(transform.position), out var p, out var f);
                return f;
            }

            return 0;
        }
    }
}