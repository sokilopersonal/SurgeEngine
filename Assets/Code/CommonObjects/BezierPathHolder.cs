using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class BezierPathHolder : ContactBase
    {
        [SerializeField] private PathData data;

        public override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            var context = ActorContext.Context;
            if (context.pathData == null)
            {
                context.pathData = data;
                
                if (data.outOfControl > 0)
                    context.flags.AddFlag(new Flag(FlagType.OutOfControl, new []{ Tags.AllowBoost }, true, data.outOfControl));
            }
            else
            {
                context.pathData = null;
            }
        }
    }
}