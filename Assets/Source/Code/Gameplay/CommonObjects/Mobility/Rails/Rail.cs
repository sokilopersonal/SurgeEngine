using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Splines;
using Zenject;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility.Rails
{
    [RequireComponent(typeof(MeshCollider), typeof(SplineContainer))]
    public class Rail : MonoBehaviour
    {
        [SerializeField] private SplineContainer container;
        [SerializeField] private DominantSpline dominant;
        [SerializeField] private float radius = 0.25f;
        public SplineContainer Container => container;
        public float Radius => radius;

        [Inject] private DiContainer _diContainer;

        private CharacterBase _character;
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponentInChildren<Collider>();
            
            if (!container)
                container = GetComponent<SplineContainer>();

            var player = _diContainer.Resolve<CharacterBase>();
            if (player.GetComponent<HomingTargetDetector>()) // Check if the player is actually able to do homing attack,
                                                             // otherwise homing target is not needed 
            {
                const string key = "HomingTargetPrefab";
                Addressables.LoadAssetAsync<GameObject>(key).Completed += op =>
                {
                    var homingTargetPrefab = op.Result.GetComponent<HomingTarget>();
                    var target = Instantiate(homingTargetPrefab, transform, false);
                    target.OnTargetReached.AddListener(AttachToRail);
                    target.SetDistanceThreshold(1f);

                    var pos = Container.Spline.EvaluatePosition(0f);
                    target.transform.position = transform.TransformPoint(pos);
                };
            }
            
            gameObject.layer = LayerMask.NameToLayer("Rail");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CharacterBase character) 
                && !IsStateExcluded(character.StateMachine.CurrentState))
            {
                AttachToRail(character);
            }
            
            bool IsStateExcluded(FState current) 
                => current is FStateRailSwitch or FStateAirObject;
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