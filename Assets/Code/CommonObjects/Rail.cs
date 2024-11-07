using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.CommonObjects
{
    public class Rail : MonoBehaviour, IPlayerContactable
    {
        public SplineContainer container;
        public float radius = 0.25f;
        
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        public void End()
        {
            _ = Common.TemporarilyDisableCollider(_collider);
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