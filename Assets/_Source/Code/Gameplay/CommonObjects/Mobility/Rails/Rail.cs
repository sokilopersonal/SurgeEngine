using NaughtyAttributes;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom.Extensions;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility.Rails
{
    [RequireComponent(typeof(MeshCollider), typeof(SplineContainer))]
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

            var pos = Container.Spline.EvaluatePosition(0f);
            HomingTarget.transform.position = transform.TransformPoint(pos);
            
            gameObject.layer = LayerMask.NameToLayer("Rail");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetActor(out CharacterBase actor) && actor.StateMachine.CurrentState is not FStateRailSwitch)
            {
                AttachToRail(actor);
            }
        }

        private void AttachToRail(CharacterBase character)
        {
            character.StateMachine.SetState<FStateGrind>()?.SetRail(this);
        }
    }
}