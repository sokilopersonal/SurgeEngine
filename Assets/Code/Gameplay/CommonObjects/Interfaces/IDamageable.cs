using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(MonoBehaviour sender, float damage);
    }
}