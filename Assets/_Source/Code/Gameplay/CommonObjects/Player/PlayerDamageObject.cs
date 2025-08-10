using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Player
{
    public class PlayerDamageObject : Entity
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CharacterBase actor))
            {
                actor.TakeDamage(this);
            }
        }
    }
}