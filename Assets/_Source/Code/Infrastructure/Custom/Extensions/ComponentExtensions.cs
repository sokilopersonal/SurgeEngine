using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Custom.Extensions
{
    public static class ComponentExtensions
    {
        public static T TryGetComponentInParent<T>(this Component instance, out T result)
        {
            if (instance.transform.parent.TryGetComponent(out result))
            {
                return result;
            }
            
            result = default;
            return default;
        }

        public static CharacterBase TryGetCharacter(this Component instance, out CharacterBase result)
        {
            if (instance.TryGetComponentInParent(out CharacterBase actor))
            {
                result = actor;
                return actor;
            }
            
            result = null;
            return null;
        }
    }
}