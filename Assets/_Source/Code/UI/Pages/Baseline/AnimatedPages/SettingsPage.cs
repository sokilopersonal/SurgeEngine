using System;
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
        private InputAction _sideAction;

        [SerializeField] private Button[] subPagesButtons;
        private int _currentSubPage;

        protected override void Awake()
        {
            base.Awake();

            _sideAction = sideActionReference.action;
        }

        private void OnEnable()
        {
            _sideAction.Enable();
            _sideAction.performed += OnSideAction;
        }

        private void OnDisable()
        {
            _sideAction.Disable();
            _sideAction.performed -= OnSideAction;
        }

        protected override void Show()
        {
            base.Show();

            _currentSubPage = 0;
            sequence.Join(background.DOFade(0.2f, enterDuration));
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

        public void Revert()
        {
            _currentOptionUI.Revert();
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
            _current = page;
        }

        private void OnSideAction(InputAction.CallbackContext obj)
        {
            if (_current == null) return;
            
            var dir = (int)obj.ReadValue<Vector2>().x;
            bool isLeft = dir < 0;
            
            _currentSubPage = isLeft ? _currentSubPage - 1 : _currentSubPage + 1;
            _currentSubPage = Mathf.Clamp(_currentSubPage, 0, subPages.Length - 1);
            SubPush(subPages[_currentSubPage]);
            
            subPagesButtons[_currentSubPage].Select();
        }
    }
}