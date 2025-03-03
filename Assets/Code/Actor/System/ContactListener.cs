using SurgeEngine.Code.CommonObjects;
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
            IPlayerContactable playerContactable = null;

            if (other.TryGetComponent(out ContactBase contactable) || other.TryGetComponent(out playerContactable)) {}
            else if (other.transform.parent != null)
            {
                other.transform.parent.TryGetComponent(out contactable);
                other.transform.parent.TryGetComponent(out playerContactable);
            }

            contactable?.Contact(other);
            playerContactable?.OnContact();
        }
    }
}