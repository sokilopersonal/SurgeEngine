using System;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects
{
    /// <summary>
    /// A class for creating hurtboxes
    /// </summary>
    public static class HurtBox
    {
        /// <summary>
        /// Creates an attached hurtbox around the transform with the given size
        /// </summary>
        /// <param name="transform">Transform to create the hurtbox around</param>
        /// <param name="size">Size of the hurtbox</param>
        /// <returns>True, if it hits anything</returns>
        public static bool CreateAttached(Entity sender, Transform transform, Vector3 size, HurtBoxTarget target)
        {
            int mask = GetMask(target);
            var hits = UnityEngine.Physics.OverlapBox(transform.position, size, transform.rotation, mask);

            foreach (var hit in hits)
            {
                Transform hitTransform = hit.transform;
                IDamageable damageable = hit.GetComponentInParent<IDamageable>();
                if (hitTransform)
                {
                    damageable?.TakeDamage(sender, 0);
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Creates an hurtbox at the given position and orientation with the given size
        /// </summary>
        /// <param name="position">Box position</param>
        /// <param name="rotation">Box rotation</param>
        /// <param name="size">Box size</param>
        /// <returns>True, if it hits anything</returns>
        public static bool Create(Entity sender, Vector3 position, Quaternion rotation, Vector3 size, HurtBoxTarget target)
        {
            int mask = GetMask(target);
            var hits = UnityEngine.Physics.OverlapBox(position, size, rotation, mask);
            
            foreach (var hit in hits)
            {
                Transform transform = hit.transform;
                IDamageable damageable = hit.GetComponentInParent<IDamageable>();
                if (transform)
                {
                    damageable?.TakeDamage(sender, 0);
                    return true;
                }
            }

            return false;
        }

        private static int GetMask(HurtBoxTarget target)
        {
            int mask = 0;
            if (target.HasFlag(HurtBoxTarget.Player))
                mask |= LayerMask.GetMask("Actor");
            if (target.HasFlag(HurtBoxTarget.Enemy))
                mask |= LayerMask.GetMask("Enemy");
            if (target.HasFlag(HurtBoxTarget.Breakable))
                mask |= LayerMask.GetMask("Breakable");
            return mask;
        }

    }

    [Flags]
    public enum HurtBoxTarget
    {
        Player = 1,
        Enemy = 2,
        Breakable = 4
    }
}