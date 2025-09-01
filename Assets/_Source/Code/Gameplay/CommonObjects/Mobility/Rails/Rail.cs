using NaughtyAttributes;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Custom.Extensions;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility.Rails
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
            if (other.transform.TryGetCharacter(out CharacterBase actor) && actor.StateMachine.CurrentState is not FStateRailSwitch or FStateSpring)
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