using System;
using System.Collections.Generic;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    [DefaultExecutionOrder(1)] // Fix character lagging behind the actual object movement
    public class MovingObject : MonoBehaviour, IPointMarkerLoader
    {
        private List<Rigidbody> _bodies;
        private Vector3 _lastPosition;
        private Vector3 _velocity;
        private Quaternion _lastRotation;
        private Quaternion _deltaRotation;

        public event Action<Rigidbody> OnBodyAdded; 

        private void Awake()
        {
            _bodies = new List<Rigidbody>();
            _lastPosition = transform.position;
            _lastRotation = transform.rotation;
        }

        private void FixedUpdate()
        {
            _velocity = (transform.position - _lastPosition) / Time.fixedDeltaTime;
            _deltaRotation = transform.rotation * Quaternion.Inverse(_lastRotation);
            _lastPosition = transform.position;
            _lastRotation = transform.rotation;

            if (_bodies.Count > 0)
            {
                for (int i = 0; i < _bodies.Count; i++)
                {
                    var body = _bodies[i];
                    body.position += _velocity * Time.fixedDeltaTime;
                    Vector3 offset = body.position - transform.position;
                    body.position = transform.position + _deltaRotation * offset;
                    body.rotation = _deltaRotation * body.rotation;
                }
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