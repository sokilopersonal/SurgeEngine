using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public class AutorunCollision : ContactBase
    {
        [SerializeField, Tooltip("How long should this trigger keep the autorun?" +
                                 "It's useful in cases where a player somehow passes through a trigger with some weird way. " +
                                 "Value less than 0 means infinity.")] 
        private float keepTime = 5;
        [SerializeField, Tooltip("How fast the player should move.")] 
        private float speed = 40f;
        [SerializeField, Tooltip("How fast the player will reach the speed.")] 
        private float easeTime = 0.5f;
        [SerializeField, Tooltip("Should this trigger end the autorun?")] 
        private bool isFinish;
        
        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            var flags = context.Flags;
            
            float dot = Vector3.Dot(context.transform.forward, transform.forward);
            if (dot < 0) return;
            
            if (!isFinish)
            {
                if (!flags.HasFlag(FlagType.Autorun))
                {
                    flags.AddFlag(new AutorunFlag(FlagType.Autorun, keepTime > 0, keepTime, speed, easeTime));
                    flags.AddFlag(new Flag(FlagType.OutOfControl, keepTime > 0, keepTime));
                }
            }
            else
            {
                flags.RemoveFlag(FlagType.Autorun);
                flags.RemoveFlag(FlagType.OutOfControl);
            }
        }
    }
}