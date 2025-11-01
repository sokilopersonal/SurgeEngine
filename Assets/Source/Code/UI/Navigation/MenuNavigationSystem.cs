using System.Collections.Generic;
using FMODUnity;
using SurgeEngine.Source.Code.UI.Animated;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace SurgeEngine.Source.Code.UI.Navigation
{
    public class MenuNavigationSystem : MonoBehaviour
    {
        [Header("Selectables")]
        [SerializeField] private List<Selectable> selectables = new List<Selectable>();
        
        [Header("Input")]
        [SerializeField] private InputActionReference navigationActionReference;

        [Header("Sound")] 
        [SerializeField] private EventReference selectSound;
        [SerializeField] private EventReference submitSound;

        private Selectable _lastSelected;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Profiler.BeginSample("Add Selectables to Navigation");
            if (selectables != null)
            {
                foreach (var autoAddSelectable in GetComponentsInChildren<AutoAddToNavigation>())
                {
                    var autoSelectable = autoAddSelectable.GetComponent<Selectable>();
                    if (!selectables.Contains(autoSelectable))
                    {
                        selectables.Add(autoSelectable);
                    }
                }
                
                foreach (var selectable in selectables)
                { 
                    SetupTrigger(selectable);
                }
            }
            Profiler.EndSample();
            
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

        private void SetupTrigger(Selectable selectable)
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
            
            var submitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Submit
            };
            submitEntry.callback.AddListener(OnSubmit);
            
            var clickEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            clickEntry.callback.AddListener(OnSubmit);
            
            var moveEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Move
            };
            moveEntry.callback.AddListener(OnMove);
            
            trigger.triggers.Add(selectEntry);
            trigger.triggers.Add(deselectEntry);
            trigger.triggers.Add(pointerEnterEntry);
            trigger.triggers.Add(pointerExitEntry);
            trigger.triggers.Add(submitEntry);
            trigger.triggers.Add(clickEntry);
            trigger.triggers.Add(moveEntry);
            
            foreach (var selectReaction in selectable.GetComponentsInChildren<SelectReaction>())
            {
                selectReaction.AddReaction(trigger);
            }
        }

        private void OnSelect(BaseEventData arg)
        {
            _lastSelected = arg.selectedObject.GetComponent<Selectable>();
            
            if (!IsActive())
                return;
            
            RuntimeManager.PlayOneShot(selectSound);
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

        private void OnSubmit(BaseEventData arg0)
        {
            if (!IsActive())
                return;
            
            RuntimeManager.PlayOneShot(submitSound);
        }

        private void OnMove(BaseEventData arg0)
        {
            
        }

        public void Select(GameObject obj, bool shouldPlaySound = false)
        {
            var current = EventSystem.current;
            if (current)
            {
                current.SetSelectedGameObject(obj);

                if (shouldPlaySound)
                {
                    RuntimeManager.PlayOneShot(selectSound);
                }
            }
        }

        public void Add(Selectable newSelectable)
        {
            if (selectables.Contains(newSelectable))
                return;
            
            selectables.Add(newSelectable);
        }
        
        private bool IsActive() => _canvasGroup.interactable && _canvasGroup.gameObject.activeInHierarchy;
    }
}