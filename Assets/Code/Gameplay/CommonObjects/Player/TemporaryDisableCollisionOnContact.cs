using System.Collections;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Player
{
    public class TemporaryDisableCollisionOnContact : MonoBehaviour
    {
        [SerializeField] private Collider[] _colliders;
        [SerializeField] private float time = 0.25f;
        
        private ContactBase contact;
        private Coroutine _disableCollisionCoroutine;
        
        private void Awake()
        {
            contact = GetComponentInParent<ContactBase>();
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
            if (_disableCollisionCoroutine != null)
            {
                StopCoroutine(_disableCollisionCoroutine);
            }
            
            _disableCollisionCoroutine = StartCoroutine(DisableCollision());
        }
        
        private IEnumerator DisableCollision()
        {
            foreach (Collider col in _colliders)
            {
                col.enabled = false;
            }
            
            yield return new WaitForSeconds(time);
            
            foreach (Collider col in _colliders)
            {
                col.enabled = true;
            }
        }
    }
}