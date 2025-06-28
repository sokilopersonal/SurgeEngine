using System;
using System.Collections.Generic;
using SurgeEngine.Code.UI.Animated;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Navigation
{
    public class MenuNavigationSystem : MonoBehaviour
    {
        [Header("Selectables")]
        [SerializeField] private List<Selectable> selectables = new List<Selectable>();
        
        [Header("Input")]
        [SerializeField] private InputActionReference navigationActionReference;

        private Selectable _lastSelected;

        private void Awake()
        {
            SetupTrigger();
            
            navigationActionReference.action.Enable();
            navigationActionReference.action.performed += _ =>
            {
                var current = EventSystem.current;
                if (current.currentSelectedGameObject == null && _lastSelected != null)
                {
                    Select(_lastSelected.gameObject);
                }
            };
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
            _lastSelected = arg.selectedObject.GetComponent<Selectable>();
        }
        
        private void OnDeselect(BaseEventData arg)
        {
            
        }
        
        private void OnPointerEnter(BaseEventData arg)
        {
            if (arg is PointerEventData pointerData)
            {
                Select(pointerData.pointerEnter, true);
            }
        }
        
        private void OnPointerExit(BaseEventData arg)
        {
            if (arg is PointerEventData pointerData)
            {
                Select(null);
            }
        }

        public void Select(GameObject obj, bool shouldPlaySound = false)
        {
            var current = EventSystem.current;
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