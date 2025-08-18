using System.Collections.Generic;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
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
                Vector3 delta = transform.position - _previousPosition;
                foreach (var body in _bodies)
                {
                    body.MovePosition(body.position + delta);
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