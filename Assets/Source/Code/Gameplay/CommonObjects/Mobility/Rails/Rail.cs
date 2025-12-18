using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes;
using SurgeEngine.Source.Code.Infrastructure.Custom.Extensions;
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
            if (other.transform.TryGetCharacter(out CharacterBase actor) && actor.StateMachine.CurrentState is not FStateRailSwitch)
            {
                AttachToRail(actor);
            }
        }

        private void AttachToRail(CharacterBase character)
        {
            Physics.IgnoreCollision(GetComponentInChildren<Collider>(), character.Kinematics.Rigidbody.GetComponent<Collider>(), true);
            character.StateMachine.SetState<FStateGrind>()?.SetRail(this, dominant);
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