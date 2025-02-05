using System;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.AeroCannon
{
    public class AeroCannonBullet : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float radius;
        [SerializeField] private LayerMask mask;
        
        private Vector3 _direction;

        private void Update()
        {
            transform.Translate(_direction * (speed * Time.deltaTime), Space.World);

            if (Physics.CheckSphere(transform.position, radius, mask))
            {
                Destroy(gameObject);
            }
        }

        public void SetDirection(Vector3 direction)
        {
            _direction = direction;
        }
    }
}