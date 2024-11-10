using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.CommonObjects
{
    public class Rail : MonoBehaviour
    {
        public SplineContainer container;
        public float radius = 0.25f;

        [SerializeField] private Transform homingTargetPrefab;
        
        private Collider[] _colliders;

        private void Awake()
        {
            _colliders = GetComponentsInChildren<Collider>();

            container.Evaluate(0f, out var start, out _, out var up);
            container.Evaluate(1f, out var end, out _, out var eUp);
            
            var startTarget = Instantiate(homingTargetPrefab, SurgeMath.Float3ToVector3(start) + SurgeMath.Float3ToVector3(up) * 0.75f, Quaternion.identity);
            startTarget.parent = transform;
            
            var endTarget = Instantiate(homingTargetPrefab, SurgeMath.Float3ToVector3(end) + SurgeMath.Float3ToVector3(eUp) * 0.75f, Quaternion.identity);
            endTarget.parent = transform;
        }

        public void End()
        {
            for (int i = 0; i < _colliders.Length; i++)
            {
                _ = Common.TemporarilyDisableCollider(_colliders[i], 0.1f);
            }
        }

        public void OnContact()
        {
            var context = ActorContext.Context;

            if (!context.stateMachine.Is<FStateGrind>())
            {
                context.stateMachine.SetState<FStateGrind>().SetRail(this);
            }
        }
    }
}