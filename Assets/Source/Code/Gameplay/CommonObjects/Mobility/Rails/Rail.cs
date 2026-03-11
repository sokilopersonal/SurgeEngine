using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility.Rails
{
    [RequireComponent(typeof(MeshCollider), typeof(SplineContainer))]
    public class Rail : MonoBehaviour
    {
        [SerializeField] private SplineContainer container;
        [SerializeField] private DominantSpline dominant;
        
        [SerializeField] private float radius = 0.25f;
        [SerializeField] private HomingTarget homingTargetPrefab;
        public SplineContainer Container => container;
        public float Radius => radius;
        public HomingTarget HomingTarget { get; private set; }

        private CharacterBase _character;
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponentInChildren<Collider>();
            
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
            if (other.TryGetComponent(out CharacterBase character) && character.StateMachine.CurrentState is not FStateRailSwitch)
            {
                AttachToRail(character);
            }
        }

        private void AttachToRail(CharacterBase character)
        {
            Physics.IgnoreCollision(_collider, character.Model.Collision, true);
            character.StateMachine.SetState<FStateGrind>()?.SetRail(this, dominant);
            _character = character;
            _character.StateMachine.OnStateAssign += DisableCollision;
        }

        private void DisableCollision(FState obj)
        {
            if (obj is not FStateGrind)
            {
                Physics.IgnoreCollision(_collider, _character.Model.Collision, false);
                _character.StateMachine.OnStateAssign -= DisableCollision;
            }
        }
    }
}