using System;
using System.Collections;
using DG.Tweening;
using SurgeEngine._Source.Code.UI.Pages.Baseline.AnimatedPages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine._Source.Code.UI
{
    public class SettingsPageNotification : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private SettingsPage settingsPage;

        private Sequence _tween;

        private void Awake()
        {
            group.alpha = 0f;
            
            settingsPage.OnPreEnter.AddListener(() =>
            {
                _tween?.Kill();
                group.alpha = 0f;
            });
        }

        private void OnEnable()
        {
            settingsPage.OnSaveAll += ShowSave;
            settingsPage.OnRevertAll += ShowRevert;
        }

        private void OnDisable()
        {
            settingsPage.OnSaveAll -= ShowSave;
            settingsPage.OnRevertAll -= ShowRevert;
        }

        private void ShowSave()
        {
            Show("Settings have been saved!");
        }

        private void ShowRevert()
        {
            Show("Settings have been restored!");
        }

        private void Show(string notification, float duration = 0.2f)
        {
            text.text = notification;
            
            _tween?.Kill();
            _tween = DOTween.Sequence();
            _tween.SetUpdate(true);
            _tween.Append(group.DOFade(1f, duration).From(0));
            _tween.AppendInterval(3f);
            _tween.Append(group.DOFade(0f, duration));
        }

        private void Hide()
        {
            _tween?.Kill();
            _tween = DOTween.Sequence();
            _tween.SetUpdate(true);
            _tween.Append(group.DOFade(0f, 0.2f));
        }
    }
}