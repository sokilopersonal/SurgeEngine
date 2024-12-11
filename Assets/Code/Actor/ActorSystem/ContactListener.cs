using System.Collections;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ContactListener : MonoBehaviour
    {
        private Collider _collider;
        private Coroutine _coroutine;

        private void Start()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ContactBase contactable)) {}
            else
            {
                if (other.transform.parent != null)
                {
                    other.transform.parent.TryGetComponent(out contactable);
                }
            }
            
            contactable?.Contact(other);
        }
        
        private IEnumerator TemporarilyDisableCollider(float duration)
        {
            _collider.enabled = false;
            yield return new WaitForSeconds(duration);
            _collider.enabled = true;
        }
    }
}