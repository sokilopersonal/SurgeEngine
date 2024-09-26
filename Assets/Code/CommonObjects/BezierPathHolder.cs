using SurgeEngine.Code.ActorSystem;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.CommonObjects
{
    public class BezierPathHolder : ContactBase
    {
        [SerializeField] private SplineContainer splineContainer;

        public override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            var context = ActorContext.Context;
            if (context.bezierPath == null)
            {
                context.bezierPath = splineContainer;
                Debug.Log("Created");
            }
            else
            {
                context.bezierPath = null;
                Debug.Log("Deleted");
            }
        }
    }
}