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
        [SerializeField] private ParticleSystem grindSparkles;
        public SplineContainer Container => container;
        public float Radius => radius;
        public HomingTarget HomingTarget { get; private set; }

        private ParticleSystem _sparkles;

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
            if (other.transform.parent.TryGetComponent(out ActorBase actor))
            {
                AttachToRail(actor);
            }
        }

        private void AttachToRail(ActorBase actor)
        {
            actor.stateMachine.SetState<FStateGrind>()?.SetRail(this);

            ClearSparkles();
            
            _sparkles = Instantiate(grindSparkles, actor.kinematics.Rigidbody.transform, false);
            _sparkles.transform.localPosition = Vector3.down;
            _sparkles.Play(true);
        }

        private void ClearSparkles()
        {
            if (_sparkles != null)
            {
                _sparkles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                _sparkles.transform.SetParent(null);
                Debug.Log("cleared");
                Destroy(_sparkles.gameObject, 1f);
            }
        }

        public void End()
        {
            ClearSparkles();
        }
    }
}