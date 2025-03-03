using System.Collections.Generic;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class MovingObject : MonoBehaviour
    {
        private List<Rigidbody> _bodies;
        private Vector3 _previousPosition;

        private void Start()
        {
            _bodies = new List<Rigidbody>();
            _previousPosition = transform.position;
        }

        private void FixedUpdate()
        {
            if (_bodies.Count > 0)
            {
                foreach (var body in _bodies)
                {
                    body.position += transform.position - _previousPosition;
                }
            }
            
            _previousPosition = transform.position;
        }
        
        public void Add(Rigidbody body)
        {
            if (!_bodies.Contains(body))
                _bodies.Add(body);
        }
        
        public void Remove(Rigidbody body)
        {
            _bodies.Remove(body);
        }
    }
}