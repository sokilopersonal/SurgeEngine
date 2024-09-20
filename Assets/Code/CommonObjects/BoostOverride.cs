using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class BoostOverride : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (ActorContext.Context.ID == other.gameObject.GetInstanceID())
            {
                Debug.Log("It's Sonic!");
            }
        }
    }
}