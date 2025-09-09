using System.Collections.Generic;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public class MovingObject : MonoBehaviour
    {
        private List<Rigidbody> _bodies;
        private Vector3 _lastPosition;
        private Vector3 _velocity;

        private void Awake()
        {
            _bodies = new List<Rigidbody>();
            _lastPosition = transform.position;
        }

        private void FixedUpdate()
        {
            _velocity = (transform.position - _lastPosition) / Time.fixedDeltaTime;
            _lastPosition = transform.position;

            if (_bodies.Count > 0)
            {
                foreach (var body in _bodies)
                    body.MovePosition(body.position + _velocity * Time.fixedDeltaTime);
            }
        }

        public void Add(Rigidbody body)
        {
            if (!_bodies.Contains(body))
                _bodies.Add(body);
        }

        public void Remove(Rigidbody body)
        {
            if (_bodies.Contains(body))
            {
                _bodies.Remove(body);
                body.linearVelocity += _velocity;
            }
        }
    }
}