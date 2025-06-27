using System;
using System.Collections.Generic;
using SurgeEngine.Code.UI.Animated;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Navigation
{
    public class MenuNavigationSystem : MonoBehaviour
    {
        [SerializeField] private List<Selectable> selectables = new List<Selectable>();

        private void Awake()
        {
            SetupTrigger();
        }

        private void SetupTrigger()
        {
            foreach (var selectable in selectables)
            {
                var trigger = selectable.GetComponent<EventTrigger>();
                if (trigger == null)
                    trigger = selectable.gameObject.AddComponent<EventTrigger>();

                var selectEntry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.Select
                };
                selectEntry.callback.AddListener(OnSelect);
                
                var deselectEntry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.Deselect
                };
                deselectEntry.callback.AddListener(OnDeselect);
                
                var pointerEnterEntry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerEnter
                };
                pointerEnterEntry.callback.AddListener(OnPointerEnter);
                
                var pointerExitEntry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerExit
                };
                pointerExitEntry.callback.AddListener(OnPointerExit);
                
                trigger.triggers.Add(selectEntry);
                trigger.triggers.Add(deselectEntry);
                trigger.triggers.Add(pointerEnterEntry);
                trigger.triggers.Add(pointerExitEntry);
                
                foreach (var selectReaction in selectable.GetComponentsInChildren<SelectReaction>())
                {
                    selectReaction.AddReaction(trigger);
                }
            }
        }

        private void OnSelect(BaseEventData arg)
        {
            
        }
        
        private void OnDeselect(BaseEventData arg)
        {
            
        }
        
        private void OnPointerEnter(BaseEventData arg)
        {
            if (arg is PointerEventData pointerData)
                Select(pointerData.pointerEnter, true);
        }
        
        private void OnPointerExit(BaseEventData arg)
        {
            if (arg is PointerEventData pointerData)
                Select(null);
        }

        public void Select(GameObject obj, bool shouldPlaySound = false)
        {
            var current = EventSystem.current;
            if (current.currentSelectedGameObject == obj)
                return;
            
            current.SetSelectedGameObject(obj);

            if (shouldPlaySound)
            {
                // Play sound
            }
        }

        public void Add(Selectable newSelectable)
        {
            if (selectables.Contains(newSelectable))
                return;
            
            selectables.Add(newSelectable);
        }
    }
}