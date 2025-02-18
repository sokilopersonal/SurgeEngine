using System;
using DG.Tweening;
using UnityEngine;

namespace SurgeEngine.Code.UI
{
    public class PlayerUI : MonoBehaviour
    {
        public static PlayerUI Instance { get; private set; }

        [Header("Definers")]
        [SerializeField] private UIDefinerComponent startMenu;
        [SerializeField] private UIDefinerComponent[] uiDefiners;

        [Header("Menu Change Settings")]
        [SerializeField] private Ease changeEase = Ease.OutCubic;
        [SerializeField] private float changeDuration = 0.3f;
        
        private Tween _tween;
        
        private UIDefinerComponent _currentMenu;

        private void Awake()
        {
            Instance = this;
            _currentMenu = startMenu;
        }

        public void OpenMenu(Menu menu)
        {
            if (_currentMenu != null && _currentMenu.menuType == menu) return;
            _tween?.Kill();
            UIDefinerComponent target = null;
            foreach (var ui in uiDefiners)
            {
                if (ui.menuType == menu)
                {
                    target = ui;
                    break;
                }
            }
            if (target == null) return;
            if (_currentMenu != null)
            {
                _tween = _currentMenu.canvasGroup.DOFade(0, changeDuration).SetEase(changeEase).OnComplete(() =>
                {
                    _currentMenu.gameObject.SetActive(false);
                    target.gameObject.SetActive(true);
                    target.canvasGroup.alpha = 0;
                    _tween = target.canvasGroup.DOFade(1, changeDuration).SetEase(changeEase).SetUpdate(true);
                    _currentMenu = target;
                }).SetUpdate(true);
            }
            else
            {
                target.gameObject.SetActive(true);
                target.canvasGroup.alpha = 0;
                _tween = target.canvasGroup.DOFade(1, changeDuration).SetEase(changeEase).SetUpdate(true);
                _currentMenu = target;
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }

    public enum Menu
    {
        Main,
        Pause,
        Options
    }
}