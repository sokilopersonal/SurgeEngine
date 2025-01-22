using System;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class DestroyedPiece : MonoBehaviour
    {
        [SerializeField] private float destroyTime = 15f;
        
        [SerializeField] private Rigidbody[] rigidbodies;
        
        public void ApplyExplosionForce(float force, Vector3 position, float radius)
        {
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].AddExplosionForce(force, position, radius);
            }
        }

        public void ApplyDirectionForce(Vector3 direction, float force)
        {
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].AddForce(direction * force, ForceMode.VelocityChange);
            }
        }

        private void Awake()
        {
            Destroy(gameObject, destroyTime);
        }
    }
}