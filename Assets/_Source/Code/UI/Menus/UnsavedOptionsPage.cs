using DG.Tweening;
using SurgeEngine.Code.Infrastructure.Tools.Managers.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Menus
{
    public class UnsavedOptionsPage : Page
    {
        [SerializeField] private RectTransform firstBox, secondBox, mask;
        [SerializeField] private Button saveButton, revertButton;
        private OptionUI optionUI;

        private float _startWidth;
        private float _startHeight;

        protected override void Awake()
        {
            base.Awake();

            optionUI = GetComponentInParent<OptionUI>();
        }

        private void Start()
        {
            _startWidth = firstBox.sizeDelta.x;
            _startHeight = firstBox.sizeDelta.y;
            
            saveButton.onClick.AddListener(CloseUnsavedPage);
            revertButton.onClick.AddListener(CloseUnsavedPage);
            
            saveButton.onClick.AddListener(() => MenusHandler.Instance.OpenMenu<OptionsPage>());
            
            saveButton.onClick.AddListener(optionUI.Save);
            revertButton.onClick.AddListener(optionUI.Revert);
        }

        protected override void InsertIntroAnimations()
        {
            // First box - black
            // Second box - cyan
            
            AnimationSequence.Join(Group.DOFade(1f, 0.2f).From(0));
            AnimationSequence.Join(SizeTween(secondBox, true));
            AnimationSequence.Join(SizeTween(firstBox, true).SetDelay(0.1f));
            AnimationSequence.Join(SizeTween(mask, true).SetDelay(0.1f));
        }

        protected override void InsertOutroAnimations()
        {
            AnimationSequence.Join(Group.DOFade(0f, 0.2f));
            AnimationSequence.Join(SizeTween(secondBox, false));
            AnimationSequence.Join(SizeTween(firstBox, false).SetDelay(0.1f));;
        }

        private Tween SizeTween(RectTransform t, bool intro)
        {
            Tween tween;
            
            if (intro)
            {
                tween = t.DOSizeDelta(new Vector2(_startWidth, _startHeight), duration).SetEase(Ease.OutCubic)
                    .From(new Vector2(0, _startHeight));
            }
            else
            {
                tween = t.DOSizeDelta(new Vector2(0, _startHeight), 0.3f).SetEase(Ease.OutCubic)
                    .From(new Vector2(_startWidth, _startHeight));
            }

            return tween;
        }

        private void CloseUnsavedPage()
        {
            CloseInternal();
        }
    }
}