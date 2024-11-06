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
            var kinematics = context.kinematics;
            if (!kinematics.IsPathValid())
            {
                kinematics.SetPath(data);
                
                if (data.outOfControl > 0)
                    context.flags.AddFlag(new Flag(FlagType.OutOfControl, new []{ Tags.AllowBoost }, true, data.outOfControl));
            }
            else
            {
                kinematics.SetPath(null);
            }
        }
    }
}