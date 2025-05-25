using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI
{
    public class MenuEventSystemHandler : MonoBehaviour
    {
        public static MenuEventSystemHandler Instance { get; private set; }
        
        [Header("References")]
        [SerializeField] private List<Selectable> selectables = new List<Selectable>();
        public List<Selectable> Selectables => selectables;
        private CanvasGroup _parentGroup;
        
        [Header("Navigation Reference")]
        [SerializeField] private InputActionReference navigationReference;
        private Selectable _lastSelectable;

        [Header("Sound")]
        [SerializeField] private EventReference selectSound;
        [SerializeField] private EventReference submitSound;

        protected void Awake()
        {
            _parentGroup = GetComponent<CanvasGroup>();
            Instance = this;
            
            foreach (var sel in selectables)
            {
                AddTriggerListeners(sel);
            }
        }

        public virtual void AddTriggerListeners(Selectable selectable)
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
            var submitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Submit
            };
            var clickEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            
            // Add triggers to EventTrigger
            trigger.triggers.Add(selectEntry);
            trigger.triggers.Add(deselectEntry);
            trigger.triggers.Add(pointerEnterEntry);
            trigger.triggers.Add(pointerExitEntry);
            trigger.triggers.Add(submitEntry);
            trigger.triggers.Add(clickEntry);
            
            // Add listeners
            selectEntry.callback.AddListener(OnSelect);
            deselectEntry.callback.AddListener(OnDeselect);
            pointerEnterEntry.callback.AddListener(OnPointerEnter);
            pointerExitEntry.callback.AddListener(OnPointerExit);
            submitEntry.callback.AddListener(OnUISubmit);
            clickEntry.callback.AddListener(OnUISubmit);
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
            if (_lastSelectable != null) EventSystem.current.SetSelectedGameObject(_lastSelectable.gameObject);
        }

        private void OnSelect(BaseEventData eventData)
        {
            var selectable = eventData.selectedObject.GetComponent<Selectable>();
            if (AllowToNavigate() && _lastSelectable != selectable)
            {
                RuntimeManager.PlayOneShot(selectSound);
            }
            
            _lastSelectable = selectable;
            SelectionBox selBox = eventData.selectedObject.GetComponentInChildren<SelectionBox>();
            if (selBox != null)
            {
                selBox.Select();
            }
        }

        private void OnDeselect(BaseEventData eventData)
        {
            SelectionBox selBox = eventData.selectedObject.GetComponentInChildren<SelectionBox>();
            if (selBox != null)
            {
                selBox.Deselect();
            }
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

        private void OnUISubmit(BaseEventData eventData)
        {
            if (AllowToNavigate())
            {
                RuntimeManager.PlayOneShot(submitSound);
                
                var selectedObject = eventData.selectedObject;
                if (selectedObject)
                {
                    SelectionBox selBox = selectedObject.GetComponentInChildren<SelectionBox>();
                    if (selBox != null)
                    {
                        selBox.Select();
                    }
                }
            }
        }

        private void OnUINavigate(InputAction.CallbackContext context)
        {
            if (EventSystem.current.currentSelectedGameObject == null && _lastSelectable != null)
            {
                EventSystem.current.SetSelectedGameObject(_lastSelectable.gameObject);
            }
        }
        
        private bool AllowToNavigate()
        {
            return _parentGroup.alpha > 0 && _parentGroup.interactable;
        }
    }
}