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
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.transform.TryGetComponent(out Rigidbody rb))
            {
                _movingObject.Remove(rb);
            }
        }
    }
}