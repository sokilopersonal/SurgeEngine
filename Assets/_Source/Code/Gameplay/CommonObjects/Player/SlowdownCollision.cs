using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Custom.Extensions;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Player
{
    public class SlowdownCollision : StageObject
    {
        [SerializeField] private float maxSpeed = 35f;

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);
            
            context.Flags.AddFlag(new SlowdownFlag(FlagType.Slowdown, false, 0, maxSpeed));
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetCharacter(out var character))
            {
                character.Flags.RemoveFlag(FlagType.Slowdown);
            }
        }
    }
}