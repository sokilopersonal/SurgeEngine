using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class Ring : ActorTrigger
    {
        [SerializeField] private float rotationSpeed = 360f;

        private void Update()
        {
            transform.rotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
        }

        protected override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            ActorEvents.OnRingCollected?.Invoke(this);
            
            gameObject.SetActive(false);
        }
    }
}