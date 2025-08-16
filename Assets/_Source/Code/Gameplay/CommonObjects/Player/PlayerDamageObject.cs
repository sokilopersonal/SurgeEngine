using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Player
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