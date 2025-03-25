using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Menus
{
    public class OptionsPage : Page
    {
        [SerializeField] private RectTransform topGradient, bottomGradient;
        
        [Header("Boxes")]
        [SerializeField] private RectTransform topSelectionBox;
        [SerializeField] private RectTransform barSelectionBox;
        
        [Header("Sub Pages")]
        [SerializeField] private RectTransform[] subPagesTexts;
        [SerializeField] private CanvasGroup[] subPages;
        
        [Header("Setting Bars")]
        [SerializeField] private SettingBar[] bars;
        
        [Header("Description")]
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private RawImage previewImage;
        
        [Header("Input")]
        [SerializeField] private InputActionReference sideInput;

        private Tween _selectionTween;
        private Tween _barSelectionTween;
        private Tween _subPageTween;
        private int _selected;
        private int _lastSelected;

        private float _startHeightTop;
        private float _startHeightBottom;

        protected override void Awake()
        {
            base.Awake();
            
            _startHeightTop = topGradient.anchoredPosition.y;
            _startHeightBottom = bottomGradient.anchoredPosition.y;

            FastSelectSubPage();
        }

        private void OnEnable()
        {
            sideInput.action.Enable();
            sideInput.action.performed += OnSideInput;

            foreach (var bar in bars)
            {
                bar.OnBarSelected += OnBarSelected;
            }
        }

        private void OnDisable()
        {
            sideInput.action.performed -= OnSideInput;
            sideInput.action.Disable();
            
            foreach (var bar in bars)
            {
                bar.OnBarSelected -= OnBarSelected;
            }
        }

        protected override void InsertIntroAnimations()
        {
            _selected = 0;
            _selectionTween?.Kill();
            topSelectionBox.anchoredPosition = subPagesTexts[_selected].anchoredPosition;
            FastSelectSubPage();
            
            AnimationSequence.Join(Group.DOFade(1, duration).From(0));
            AnimationSequence.Join(topGradient.DOAnchorPosY(_startHeightTop, duration * 2).SetEase(Ease.OutCubic).From(new Vector2(0, topGradient.sizeDelta.y)));
            AnimationSequence.Join(bottomGradient.DOAnchorPosY(_startHeightBottom, duration * 2).SetEase(Ease.OutCubic).From(new Vector2(0, -bottomGradient.sizeDelta.y)));
        }

        protected override void InsertOutroAnimations()
        {
            AnimationSequence.Join(Group.DOFade(0, duration).From(1));
            AnimationSequence.Join(topGradient.DOAnchorPosY(topGradient.sizeDelta.y, duration).SetEase(Ease.OutCubic).From(new Vector2(0, _startHeightTop)));
            AnimationSequence.Join(bottomGradient.DOAnchorPosY(-bottomGradient.sizeDelta.y, duration).SetEase(Ease.OutCubic).From(new Vector2(0, _startHeightBottom)));
        }

        private void OnSideInput(InputAction.CallbackContext obj)
        {
            if (Active)
            {
                _selected += (int)obj.ReadValue<Vector2>().x;
                _selected = Mathf.Clamp(_selected, 0, subPages.Length - 1);
                
                if (_lastSelected == _selected) return;
                
                _subPageTween?.Kill(true);
                _subPageTween = subPages[_lastSelected].DOFade(0, duration / 2).SetEase(Ease.OutCubic).SetUpdate(true);
                
                if (_selected == _lastSelected) return;
                _lastSelected = _selected;
                
                _selectionTween?.Kill(true);
                _selectionTween = topSelectionBox.DOAnchorPos(subPagesTexts[_selected].anchoredPosition, duration / 2).SetEase(Ease.OutCubic).SetUpdate(true);
                
                _subPageTween?.Kill(true);
                _subPageTween = subPages[_selected].DOFade(1, duration / 2).SetEase(Ease.OutCubic).SetUpdate(true).OnComplete(FastSelectSubPage);
            }
        }

        private void FastSelectSubPage()
        {
            for (int i = 0; i < subPages.Length; i++)
            {
                subPages[i].alpha = _selected == i ? 1 : 0;
                subPages[i].interactable = _selected == i;
                subPages[i].blocksRaycasts = _selected == i;
            }
        }

        private void OnBarSelected(SettingBar bar)
        {
            RectTransform rect = bar.transform as RectTransform;
            _barSelectionTween?.Kill();
            _barSelectionTween = barSelectionBox.DOAnchorPosY(rect.anchoredPosition.y, 0.1f).SetEase(Ease.OutCubic).SetUpdate(true);

            descriptionText.text = bar.Description;
        }
    }
}