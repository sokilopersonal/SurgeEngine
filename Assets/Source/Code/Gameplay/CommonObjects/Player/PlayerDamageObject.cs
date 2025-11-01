using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Player
{
    public class PlayerDamageObject : Entity
    {
        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out CharacterBase character))
            {
                DamagePlayer(character);
            }
        }

        protected virtual void DamagePlayer(CharacterBase character)
        {
            character.Life.TakeDamage(this);
        }
    }
}