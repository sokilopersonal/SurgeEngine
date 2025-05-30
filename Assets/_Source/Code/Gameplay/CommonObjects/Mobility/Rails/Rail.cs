using System;
using NaughtyAttributes;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility.Rails
{
    [RequireComponent(typeof(MeshCollider), typeof(SplineContainer), typeof(SplineExtrude))]
    public class Rail : MonoBehaviour
    {
        private SplineContainer container;
        
        [SerializeField] private float radius = 0.25f;
        [SerializeField, Required] private HomingTarget homingTargetPrefab;
        public SplineContainer Container => container;
        public float Radius => radius;
        public HomingTarget HomingTarget { get; private set; }

        private void Awake()
        {
            if (!container)
                container = GetComponent<SplineContainer>();
            
            HomingTarget = Instantiate(homingTargetPrefab, transform, false);
            HomingTarget.OnTargetReached.AddListener(AttachToRail);
            HomingTarget.SetDistanceThreshold(1f);
            
            gameObject.layer = LayerMask.NameToLayer("Rail");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent(out ContactListener listener))
            {
                AttachToRail(listener.Actor);
            }
        }

        private void AttachToRail(ActorBase actor)
        {
            actor.StateMachine.SetState<FStateGrind>()?.SetRail(this);
        }
    }
}