using System;
using System.Collections.Generic;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public class MovingObject : MonoBehaviour, IPointMarkerLoader
    {
        private List<Rigidbody> _bodies;
        private Vector3 _lastPosition;
        private Vector3 _velocity;

        public event Action<Rigidbody> OnBodyAdded; 

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
            {
                _bodies.Add(body);
                
                OnBodyAdded?.Invoke(body);
            }
        }

        public void Remove(Rigidbody body)
        {
            if (_bodies.Contains(body))
            {
                _bodies.Remove(body);
                body.linearVelocity += _velocity;
            }
        }

        public void Load()
        {
            _bodies.Clear();
        }
    }
}