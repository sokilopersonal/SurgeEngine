using System.Collections;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.CommonObjects
{
    public class Rail : ContactBase
    {
        public SplineContainer container;
        public float radius = 0.25f;

        [SerializeField] private HomingTarget startTarget;
        [SerializeField] private HomingTarget endTarget;
        
        private Collider[] _colliders;
        private Coroutine _coroutine;

        private void Awake()
        {
            _colliders = GetComponentsInChildren<Collider>();

            container.Evaluate(0f, out var start, out _, out var up);
            container.Evaluate(1f, out var end, out _, out var eUp);

            startTarget.transform.position = SurgeMath.Float3ToVector3(start) + SurgeMath.Float3ToVector3(up) * 0.75f;
            endTarget.transform.position = SurgeMath.Float3ToVector3(end) + SurgeMath.Float3ToVector3(eUp) * 0.75f;
        }

        public void End()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            
            //_coroutine = StartCoroutine(DisableCollision());
        }

        public override void Contact(Collider msg)
        {
            base.Contact(msg);
            
            AttachToRail();
        }

        public void AttachToRail()
        {
            var context = ActorContext.Context;
            context.stateMachine.SetState<FStateGrind>().SetRail(this);
        }

        private IEnumerator DisableCollision(float duration = 0.1f)
        {
            foreach (var collision in _colliders)
            {
                collision.enabled = false;
            }
            
            yield return new WaitForSeconds(duration);
            
            foreach (var collision in _colliders)
            {
                collision.enabled = true;
            }
        }
    }
}