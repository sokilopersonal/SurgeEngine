using System;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    [RequireComponent(typeof(MeshCollider))]
    public class Rail : MonoBehaviour
    {
        [SerializeField] private SplineContainer container;
        [SerializeField] private float radius = 0.25f;
        [SerializeField] private HomingTarget homingTargetPrefab;
        public SplineContainer Container => container;
        public float Radius => radius;
        public HomingTarget HomingTarget { get; private set; }

        private void Awake()
        {
            HomingTarget = Instantiate(homingTargetPrefab, transform, false);
            HomingTarget.OnTargetReached.AddListener(AttachToRail);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent(out ActorBase _))
            {
                var contact = other.contacts[0];

                SplineUtility.GetNearestPoint(container.Spline, contact.point, out _, out var f);
                var up = container.Spline.EvaluateUpVector(f);
                
                float dot = Vector3.Dot(-contact.normal, up);

                if (dot >= 0.6f)
                {
                    AttachToRail();
                }
            }
        }

        private void AttachToRail()
        {
            ActorBase context = ActorContext.Context;
            context.stateMachine.SetState<FStateGrind>()?.SetRail(this);
        }
    }
}