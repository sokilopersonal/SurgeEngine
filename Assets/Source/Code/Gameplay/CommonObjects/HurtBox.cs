using System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Interfaces;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    /// <summary>
    /// A class for creating hurtboxes
    /// </summary>
    public static class HurtBox
    {
        /// <summary>
        /// Creates an attached hurtbox around the transform with the given size
        /// </summary>
        /// <param name="sender">The sender of the hurt box</param>
        /// <param name="transform">Transform to create the hurtbox around</param>
        /// <param name="offset">Position offset</param>
        /// <param name="size">Size of the hurtbox</param>
        /// <param name="target">What hurt box can hit?</param>
        /// <returns>True, if it hits the target</returns>
        public static bool CreateAttached(MonoBehaviour sender, Transform transform, Vector3 offset, Vector3 size, HurtBoxTarget target)
        {
            int mask = GetMask(target);
            var transformedOffset = transform.TransformVector(offset);
            var hits = Physics.OverlapBox(transform.position + transformedOffset, size, transform.rotation, mask);

            foreach (var hit in hits)
            {
                Transform hitTransform = hit.transform;
                IDamageable damageable = hit.GetComponentInParent<IDamageable>();
                if (hitTransform)
                {
                    damageable?.TakeDamage(sender);
                    return true;
                }
            }
            
            return false;
        }

        private static int GetMask(HurtBoxTarget target)
        {
            int mask = 0;
            if (target.HasFlag(HurtBoxTarget.Player))
                mask |= LayerMask.GetMask("Character");
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