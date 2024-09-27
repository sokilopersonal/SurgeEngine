using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class TemporaryDisableCollisionOnContact : MonoBehaviour
    {
        [SerializeField] private Collider[] _colliders;
        [SerializeField] private float time = 0.25f;
        
        private ContactBase contact;
        
        private void Awake()
        {
            contact = GetComponent<ContactBase>();
        }

        private void OnEnable()
        {
            contact.OnContact += OnContact;
        }
        
        private void OnDisable()
        {
            contact.OnContact -= OnContact;
        }

        private void OnContact(ContactBase obj)
        {
            foreach (var collision in _colliders)
            {
                _ = Common.TemporarilyDisableCollider(collision, Mathf.Abs(time));
            }
        }
    }
}