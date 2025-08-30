using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
{
    public class PhysicsPush : MonoBehaviour
    {
        [SerializeField] private LayerMask pushableLayers;
        
        private Rigidbody _selfRigidbody;

        private void Awake()
        {
            _selfRigidbody = GetComponentInParent<Rigidbody>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody && pushableLayers == (pushableLayers | (1 << other.gameObject.layer)))
            {
                other.attachedRigidbody.linearVelocity = _selfRigidbody.linearVelocity;
            }
        }
    }
}