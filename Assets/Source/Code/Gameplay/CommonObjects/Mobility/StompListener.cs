using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
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
