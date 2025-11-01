using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Player;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.PhysicsObjects
{
    public class Thorn : PlayerDamageObject
    {
        protected override void DamagePlayer(CharacterBase character)
        {
            Vector3 dir = (character.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.up, dir);
            if (dot > 0.5f)
            {
                base.DamagePlayer(character);
            }
        }
    }
}