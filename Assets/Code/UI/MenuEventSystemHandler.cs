using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI
{
    public class MenuEventSystemHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<Selectable> selectables = new List<Selectable>();
        
        [Header("Navigation Reference")]
        [SerializeField] private InputActionReference navigationReference;
        private Selectable _lastSelectable;

        [Header("Sound")]
        [SerializeField] private EventReference selectSound;

        protected void Awake()
        {
            foreach (var sel in selectables)
            {
                AddTriggerListeners(sel);
            }
        }

        protected virtual void AddTriggerListeners(Selectable selectable)
        {
            var trigger = selectable.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = selectable.gameObject.AddComponent<EventTrigger>();
            }
            
            // Define entries
            var selectEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Select
            };
            var deselectEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Deselect
            };
            var pointerEnterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            var pointerExitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            
            // Add triggers to EventTrigger
            trigger.triggers.Add(selectEntry);
            trigger.triggers.Add(deselectEntry);
            trigger.triggers.Add(pointerEnterEntry);
            trigger.triggers.Add(pointerExitEntry);
            
            // Add listeners
            selectEntry.callback.AddListener(OnSelect);
            deselectEntry.callback.AddListener(OnDeselect);
            pointerEnterEntry.callback.AddListener(OnPointerEnter);
            pointerExitEntry.callback.AddListener(OnPointerExit);
        }

        private void OnEnable()
        {
            navigationReference.action.performed += OnUINavigate;
            
            StartCoroutine(SelectAfterAFrame());
        }

        private void OnDisable()
        {
            navigationReference.action.performed -= OnUINavigate;
        }

        private IEnumerator SelectAfterAFrame()
        {
            yield return null;
            EventSystem.current.SetSelectedGameObject(_lastSelectable.gameObject);
        }

        private void OnSelect(BaseEventData eventData)
        {
            _lastSelectable = eventData.selectedObject.GetComponent<Selectable>();
            
            RuntimeManager.PlayOneShot(selectSound);
        }

        private void OnDeselect(BaseEventData eventData)
        {
            
        }

        private void OnPointerEnter(BaseEventData eventData)
        {
            if (eventData is PointerEventData data)
            {
                Selectable sel = data.pointerEnter.GetComponentInParent<Selectable>();
                if (sel == null)
                {
                    sel = data.pointerEnter.GetComponentInChildren<Selectable>();
                }
                
                data.selectedObject = sel.gameObject;
            }
        }

        private void OnPointerExit(BaseEventData eventData)
        {
            if (eventData is PointerEventData data)
            {
                data.selectedObject = null;
            }
        }
        
        private void OnUINavigate(InputAction.CallbackContext context)
        {
            if (EventSystem.current.currentSelectedGameObject == null && _lastSelectable != null)
            {
                EventSystem.current.SetSelectedGameObject(_lastSelectable.gameObject);
            }
        }
    }
}