using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class StompListener : MonoBehaviour, IStompHandler
    {
        public UnityEvent OnContact;
        
        public void OnStomp()
        {
            OnContact.Invoke();
        }
    }
}
