using UnityEngine;
using UnityEngine.EventSystems;

namespace SurgeEngine._Source.Code.UI.Animated
{
    public abstract class SelectReaction : MonoBehaviour
    {
        public void AddReaction(EventTrigger trigger)
        {
            var selectEntry = trigger.triggers.Find(x => x.eventID == EventTriggerType.Select);
            selectEntry.callback.AddListener(OnSelect);
            var deselectEntry = trigger.triggers.Find(x => x.eventID == EventTriggerType.Deselect);
            deselectEntry.callback.AddListener(OnDeselect);
            var submitEntry = trigger.triggers.Find(x => x.eventID == EventTriggerType.Submit);
            submitEntry.callback.AddListener(OnSubmit);
            var clickEntry = trigger.triggers.Find(x => x.eventID == EventTriggerType.PointerClick);
            clickEntry.callback.AddListener(OnClick);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
            
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            
        }
        
        public virtual void OnClick(BaseEventData eventData)
        {
            
        }
    }
}