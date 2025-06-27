using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SurgeEngine.Code.UI.Pages.Baseline
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class PageController : MonoBehaviour
    {
        [SerializeField] private Page initial;
        [SerializeField] private GameObject firstSelectedObject;
        
        private Stack<Page> _pageStack;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _pageStack = new Stack<Page>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            if (firstSelectedObject)
                EventSystem.current.SetSelectedGameObject(firstSelectedObject);

            if (initial)
                Push(initial);
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
                Debug.LogWarning("Trying to pop a page, but only 1 page remains in the page stack.");
            }
        }

        public void PopAllPages()
        {
            foreach (var page in _pageStack)
            {
                Pop();
            }
        }
        
        // Input methods
        public void OnCancel()
        {
            if (_canvasGroup.interactable && _canvasGroup.gameObject.activeInHierarchy)
            {
                if (_pageStack.Count != 0)
                    Pop();
            }
        }
        
        public bool IsPageInStack(Page page) => _pageStack.Contains(page);
        public bool IsPageOnTop(Page page) => _pageStack.Count > 0 && _pageStack.Peek() == page;
    }
}