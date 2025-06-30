using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SurgeEngine.Code.UI.Animated
{
    public abstract class SelectReaction : MonoBehaviour
    {
        public void AddReaction(EventTrigger trigger)
        {
            var selectEntry = trigger.triggers.Find(x => x.eventID == EventTriggerType.Select);
            selectEntry.callback.AddListener(OnSelect);
            var deselectEntry = trigger.triggers.Find(x => x.eventID == EventTriggerType.Deselect);
            deselectEntry.callback.AddListener(OnDeselect);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            
        }

        public virtual void OnDeselect(BaseEventData eventData)
        {
            
        }
    }
}