using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility.Rails
{
    [RequireComponent(typeof(MeshCollider), typeof(SplineContainer), typeof(SplineExtrude))]
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
            if (!container)
                container = GetComponent<SplineContainer>();
            
            HomingTarget = Instantiate(homingTargetPrefab, transform, false);
            HomingTarget.OnTargetReached.AddListener(AttachToRail);
            
            gameObject.layer = LayerMask.NameToLayer("Rail");
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent(out ActorBase actor) && !actor.stateMachine.GetState<FStateGrind>().IsRailCooldown())
            {
                AttachToRail();
            }
        }

        private void AttachToRail()
        {
            ActorBase context = ActorContext.Context;
            context.stateMachine.SetState<FStateGrind>()?.SetRail(this);
        }
    }
}