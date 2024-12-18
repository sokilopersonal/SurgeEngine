using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class BezierPathHolder : ContactBase
    {
        [SerializeField] private PathData data;
        [SerializeField] private bool stayInside;

        private bool _playerInside;

        private void Update()
        {
            if (_playerInside)
            {
                Actor context = ActorContext.Context;
                ActorKinematics kinematics = context.kinematics;
                kinematics.SetPath(data);
            }
        }

        public override void Contact(Collider msg)
        {
            base.Contact(msg);

            if (!stayInside)
            {
                TogglePath();
            }
            else
            {
                _playerInside = true;
            }
        }
        
        private void TogglePath()
        {
            Actor context = ActorContext.Context;
            ActorKinematics kinematics = context.kinematics;
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