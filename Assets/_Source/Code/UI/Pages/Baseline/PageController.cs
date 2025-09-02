using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SurgeEngine._Source.Code.UI.Pages.Baseline
{
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class PageController : MonoBehaviour
    {
        [Header("Initial")]
        [SerializeField] protected Page initial;
        [SerializeField] private GameObject firstSelectedObject;
        
        [Header("Input")]
        [SerializeField] protected InputActionReference cancelActionReference;
        [SerializeField] private UnityEvent onCancelEvent;
        
        public int Count => _pageStack.Count;

        protected Stack<Page> _pageStack;
        protected CanvasGroup _canvasGroup;

        protected virtual void Awake()
        {
            _pageStack = new Stack<Page>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected virtual void Start()
        {
            if (firstSelectedObject)
                EventSystem.current.SetSelectedGameObject(firstSelectedObject);

            if (initial)
                Push(initial);
        }

        protected virtual void OnEnable()
        {
            if (cancelActionReference)
            {
#if UNITY_EDITOR
                cancelActionReference.action.ApplyBindingOverride("<Keyboard>/tab", null, "<Keyboard>/escape");
#endif
                
                cancelActionReference.action.Enable();
                cancelActionReference.action.performed += OnCancelAction;
            }
        }

        protected virtual void OnDisable()
        {
            if (cancelActionReference)
            {
                cancelActionReference.action.Disable();
                cancelActionReference.action.performed -= OnCancelAction;
            }
        }

        public void Push(Page page)
        {
            page.Enter();

            if (_pageStack.Count > 0)
            {
                var current = _pageStack.Peek();
                
                if (current.ExitOnNewPush)
                    current.Exit();
            }
            
            _pageStack.Push(page);
        }

        public void Pop()
        {
            if (_pageStack.Count > 1)
            {
                var page = _pageStack.Pop();
                page.Exit();
                
                var newPage = _pageStack.Peek();
                if (newPage.ExitOnNewPush)
                    newPage.Enter();
            }
            else
            {
                if (IsActive()) Debug.LogWarning("Trying to pop a page, but only 1 page remains in the page stack.");
            }
        }

        public void PopAllPages()
        {
            for (var i = 1; i < _pageStack.Count; i++)
            {
                Pop();
            }
        }
        
        public void OnCancel()
        {
            if (IsActive())
            {
                if (_pageStack.Count != 0)
                    Pop();
            }
        }

        protected virtual void OnCancelAction(InputAction.CallbackContext obj)
        {
            onCancelEvent.Invoke();
        }

        private bool IsActive()
        {
            return _canvasGroup.interactable && _canvasGroup.gameObject.activeInHierarchy;
        }

        public bool IsPageInStack(Page page) => _pageStack.Contains(page);
        public bool IsPageOnTop(Page page) => _pageStack.Count > 0 && _pageStack.Peek() == page;
    }
}