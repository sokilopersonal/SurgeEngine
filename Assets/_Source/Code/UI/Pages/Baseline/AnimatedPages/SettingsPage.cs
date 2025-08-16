using System;
using System.Linq;
using DG.Tweening;
using SurgeEngine.Code.Infrastructure.Tools.Managers.UI;
using SurgeEngine.Code.UI.Animated;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Pages.Baseline.AnimatedPages
{
    public class SettingsPage : Page
    {
        [SerializeField] private Page[] subPages;
        private Page _current;
        private OptionUI _currentOptionUI;
        
        [SerializeField] private Image background;
        [SerializeField] private InputActionReference sideActionReference;
        [SerializeField] private InputActionReference applyActionReference;
        [SerializeField] private InputActionReference resetActionReference;
        private InputAction _sideAction;
        private InputAction _applyAction;
        private InputAction _resetAction;

        [SerializeField] private Button applyButton;
        [SerializeField] private Button resetButton;
        
        [SerializeField] private Button[] subPagesButtons;
        [SerializeField] private OptionUI[] subPagesOptionUIs;
        [SerializeField] private ConfirmWindow confirmWindow;
        private int _currentSubPage;
        private PageController _controller;

        protected override void Awake()
        {
            base.Awake();
            
            _controller = GetComponentInParent<PageController>();

            _sideAction = sideActionReference.action;
            _applyAction = applyActionReference.action;
            _resetAction = resetActionReference.action;
        }

        private void OnEnable()
        {
            _sideAction.Enable();
            _applyAction.Enable();
            _resetAction.Enable();
            _sideAction.performed += OnSideAction;
            _applyAction.performed += OnApplyAction;
            _resetAction.performed += OnResetAction;
        }

        private void OnDisable()
        {
            _sideAction.Disable();
            _applyAction.Disable();
            _resetAction.Disable();
            _sideAction.performed -= OnSideAction;
            _applyAction.performed -= OnApplyAction;
            _resetAction.performed -= OnResetAction;
        }

        protected override void Show()
        {
            base.Show();

            _currentSubPage = 0;
            sequence.Join(background.DOFade(0f, enterDuration));
        }

        protected override void Hide()
        {
            base.Hide();
            
            sequence.Join(background.DOFade(1f, exitDuration));
            _current = null;
            _currentOptionUI = null;
        }

        public void Save()
        {
            _currentOptionUI.Save();
        }
        
        public void SaveAll()
        {
            foreach (var optionUI in subPagesOptionUIs)
            {
                optionUI.Save();
            }
        }

        public void Revert()
        {
            _currentOptionUI.Revert();
        }
        
        public void RevertAll()
        {
            foreach (var optionUI in subPagesOptionUIs)
            {
                optionUI.Revert();
            }
        }

        public void CheckDirty()
        {
            if (subPagesOptionUIs.Any(x => x.IsDirty))
            {
                _controller.Push(confirmWindow);
                confirmWindow.Create(() =>
                {
                    SaveAll();
                    _controller.Pop();
                }, () =>
                {
                    RevertAll();
                    confirmWindow.Exit();
                }, RevertAll);
            }
        }

        public void SubPush(Page page)
        {
            if (_current == page) return;
            _current = page;
            _currentOptionUI = page.GetComponent<OptionUI>();
            
            foreach (var subPage in subPages)
            {
                subPage.CanvasGroup.alpha = 0f;
                subPage.CanvasGroup.interactable = false;
                subPage.CanvasGroup.blocksRaycasts = false;
            }

            page.CanvasGroup.alpha = 1f;
            page.CanvasGroup.interactable = true;
            page.CanvasGroup.blocksRaycasts = true;
        }

        private void OnSideAction(InputAction.CallbackContext obj)
        {
            if (!IsActive()) return;
            
            var dir = (int)obj.ReadValue<Vector2>().x;
            bool isLeft = dir < 0;
            
            _currentSubPage = isLeft ? _currentSubPage - 1 : _currentSubPage + 1;
            _currentSubPage = Mathf.Clamp(_currentSubPage, 0, subPages.Length - 1);
            SubPush(subPages[_currentSubPage]);
            
            subPagesButtons[_currentSubPage].Select();
        }

        private void OnApplyAction(InputAction.CallbackContext obj)
        {
            if (!IsActive()) return;
            
            applyButton.Select();
        }

        private void OnResetAction(InputAction.CallbackContext obj)
        {
            if (!IsActive()) return;
            
            resetButton.Select();
        }
    }
}