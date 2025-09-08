using SurgeEngine._Source.Code.Infrastructure.Custom.Extensions;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public class MovingObjectSensor : MonoBehaviour
    {
        private MovingObject _movingObject;

        private void Start()
        {
            _movingObject = GetComponentInParent<MovingObject>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent(out Rigidbody rb))
            {
                _movingObject.Add(rb);
            }

            if (other.transform.TryGetComponentInParent(out Rigidbody parentBody))
            {
                _movingObject.Add(parentBody);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.transform.TryGetComponent(out Rigidbody rb))
            {
                _movingObject.Remove(rb);
            }
            
            if (other.transform.TryGetComponentInParent(out Rigidbody parentBody))
            {
                _movingObject.Remove(parentBody);
            }
        }
    }
}