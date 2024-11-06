using SurgeEngine.Code.CommonObjects;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ContactListener : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent(out IPlayerContactable contactable) || other.transform.parent.TryGetComponent(out contactable))
            {
                contactable.OnContact();
            }
        }
    }
}