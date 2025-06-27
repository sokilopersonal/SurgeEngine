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

        public static ActorBase TryGetActor(this Component instance, out ActorBase result)
        {
            if (instance.TryGetComponentInParent(out ActorBase actor))
            {
                result = actor;
                return actor;
            }
            
            result = null;
            return null;
        }
    }
}