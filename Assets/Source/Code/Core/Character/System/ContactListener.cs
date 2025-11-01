using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    /// <summary>
    /// Listens for collisions
    /// </summary>
    public class ContactListener : CharacterComponent
    {
        private void OnTriggerEnter(Collider other)
        {
            /*if (other.TryGetComponent(out IPlayerContactable playerContactable)) {}
            else if (other.transform.parent != null)
            {
                other.transform.TryGetComponentInParent(out playerContactable);
            }
        
            var col = GetComponent<Collider>();
            playerContactable?.OnContact(col);*/
        }
    }
}