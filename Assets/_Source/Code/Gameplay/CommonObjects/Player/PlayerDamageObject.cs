using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Player
{
    public class PlayerDamageObject : Entity
    {
        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out CharacterBase character))
            {
                character.Life.TakeDamage(this);
            }
        }
    }
}