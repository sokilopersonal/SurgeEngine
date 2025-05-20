using System.Collections;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class Rail : MonoBehaviour
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

            container.Evaluate(0f, out float3 start, out _, out float3 up);
            container.Evaluate(1f, out float3 end, out _, out float3 eUp);

            startTarget.transform.position = SurgeMath.Float3ToVector3(start) + SurgeMath.Float3ToVector3(up) * 0.75f;
            endTarget.transform.position = SurgeMath.Float3ToVector3(end) + SurgeMath.Float3ToVector3(eUp) * 0.75f;
        }

        public void End()
        {
        }

        public void AttachToRail()
        {
            ActorBase context = ActorContext.Context;
            context.stateMachine.SetState<FStateGrind>();
            context.stateMachine.GetState<FStateGrind>().SetRail(this);
        }

        private IEnumerator DisableCollision(float duration = 0.1f)
        {
            foreach (Collider collision in _colliders)
            {
                collision.enabled = false;
            }
            
            yield return new WaitForSeconds(duration);
            
            foreach (Collider collision in _colliders)
            {
                collision.enabled = true;
            }
        }
    }
}