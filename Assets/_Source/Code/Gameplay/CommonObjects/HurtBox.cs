using System;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using UnityEngine;
using SurgeEngine.Code.Infrastructure.Custom.Extensions;

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
        /// <param name="sender">The sender of the hurt box</param>
        /// <param name="transform">Transform to create the hurtbox around</param>
        /// <param name="offset">Position offset</param>
        /// <param name="size">Size of the hurtbox</param>
        /// <param name="target">What hurt box can hit?</param>
        /// <returns>True, if it hits the target</returns>
        public static bool CreateAttached(MonoBehaviour sender, Transform transform, Vector3 offset, Vector3 size, HurtBoxTarget target)
        {
            int mask = GetMask(target);
            var hits = Physics.OverlapBox(transform.position + offset, size, transform.rotation, mask);
            
            DebugExtensions.DrawCube(Matrix4x4.TRS(transform.position + offset, transform.rotation, Vector3.one), size, Color.red);

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