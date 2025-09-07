using NaughtyAttributes;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using SurgeEngine._Source.Code.Infrastructure.Custom.Extensions;
using UnityEngine;
using UnityEngine.Splines;
using NotImplementedException = System.NotImplementedException;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility.Rails
{
    [RequireComponent(typeof(MeshCollider), typeof(SplineContainer))]
    public class Rail : MonoBehaviour
    {
        private SplineContainer _container;
        
        [SerializeField] private float radius = 0.25f;
        [SerializeField, Required] private HomingTarget homingTargetPrefab;
        public SplineContainer Container => _container;
        public float Radius => radius;
        public HomingTarget HomingTarget { get; private set; }

        private CharacterBase _character;

        private void Awake()
        {
            if (!_container)
                _container = GetComponent<SplineContainer>();
            
            HomingTarget = Instantiate(homingTargetPrefab, transform, false);
            HomingTarget.OnTargetReached.AddListener(AttachToRail);
            HomingTarget.SetDistanceThreshold(1f);

            var pos = Container.Spline.EvaluatePosition(0f);
            HomingTarget.transform.position = transform.TransformPoint(pos);
            
            gameObject.layer = LayerMask.NameToLayer("Rail");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetCharacter(out CharacterBase actor) && actor.StateMachine.CurrentState is not FStateRailSwitch)
            {
                AttachToRail(actor);
            }
        }

        private void AttachToRail(CharacterBase character)
        {
            Physics.IgnoreCollision(GetComponentInChildren<Collider>(), character.Kinematics.Rigidbody.GetComponent<Collider>(), true);
            character.StateMachine.SetState<FStateGrind>()?.SetRail(this);
            _character = character;
            _character.StateMachine.OnStateAssign += DisableCollision;
        }

        private void DisableCollision(FState obj)
        {
            if (obj is not FStateGrind)
            {
                Physics.IgnoreCollision(GetComponentInChildren<Collider>(), _character.Kinematics.Rigidbody.GetComponent<Collider>(), false);
                _character.StateMachine.OnStateAssign -= DisableCollision;
            }
        }
    }
}