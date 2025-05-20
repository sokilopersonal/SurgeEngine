using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SurgeEngine.Code.UI
{
    public class PlaySoundOnSelect : MonoBehaviour, ISelectHandler, IPointerEnterHandler
    {
        [SerializeField] private EventReference sound;
        
        public void OnSelect(BaseEventData eventData)
        {
            RuntimeManager.PlayOneShot(sound);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            RuntimeManager.PlayOneShot(sound);
        }
    }
}