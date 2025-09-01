using System;
using DG.Tweening;
using SurgeEngine._Source.Code.UI.Pages.Baseline;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine._Source.Code.UI
{
    public class ConfirmWindow : Page
    {
        [SerializeField] private RectTransform container;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        private Action _onCloseAction;
        private bool _buttonClicked;

        protected override void Show()
        {
            base.Show();
            
            sequence.Join(container.DOAnchorPosX(50, enterDuration).From(new Vector2(-1000, 0)));
        }
        
        protected override void Hide()
        {
            base.Hide();
            
            sequence.Join(container.DOAnchorPosX(-1000, exitDuration));
            if (!_buttonClicked && _onCloseAction != null)
            {
                var handler = _onCloseAction;
                _onCloseAction = null; 
                handler.Invoke();
            }
        }

        public void Create(Action confirmAction, Action cancelAction, Action onCloseAction)
        {
            Destroy();
            _buttonClicked = false;
            _onCloseAction = onCloseAction;
            confirmButton.onClick.AddListener(() => {
                _buttonClicked = true;
                confirmAction?.Invoke();
            });
            cancelButton.onClick.AddListener(() => {
                _buttonClicked = true;
                cancelAction?.Invoke();
            });
        }

        private void Destroy()
        {
            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
        }
    }
}