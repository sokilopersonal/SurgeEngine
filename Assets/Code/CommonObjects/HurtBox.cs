using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
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
        public static bool CreateAttached(Transform transform, Vector3 size)
        {
            int mask = GetMask();
            var hits = Physics.OverlapBox(transform.position, size, transform.rotation, mask);

            foreach (var hit in hits)
            {
                if (hit.transform.parent.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(0);
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
        public static bool Create(Vector3 position, Quaternion rotation, Vector3 size)
        {
            int mask = GetMask();
            var hits = Physics.OverlapBox(position, size, rotation, mask);
            
            foreach (var hit in hits)
            {
                if (hit.transform.parent.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(0);
                    return true;
                }
            }

            return false;
        }

        private static int GetMask()
        {
            return LayerMask.GetMask("Enemy", "Breakable");
        } 
    }
}