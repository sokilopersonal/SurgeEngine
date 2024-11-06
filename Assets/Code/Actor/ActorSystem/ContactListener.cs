using SurgeEngine.Code.CommonObjects;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ContactListener : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IPlayerContactable contactable)) {}
            else
            {
                if (other.transform.parent != null)
                {
                    other.transform.parent.TryGetComponent(out contactable);
                }
            }
            
            contactable?.OnContact();
        }
    }
}