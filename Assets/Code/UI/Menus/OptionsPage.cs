using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.UI.Menus
{
    public class OptionsPage : Page
    {
        [SerializeField] private RectTransform topGradient, bottomGradient;
        
        [SerializeField] private RectTransform selectionBox;
        [SerializeField] private RectTransform[] subPagesTexts;
        [SerializeField] private CanvasGroup[] subPages;
        [SerializeField] private InputActionReference sideInput;

        private Tween _selectionTween;
        private int _selected;

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
        }

        private void OnDisable()
        {
            sideInput.action.performed -= OnSideInput;
            sideInput.action.Disable();
        }

        protected override void InsertIntroAnimations()
        {
            _selected = 0;
            _selectionTween?.Kill();
            selectionBox.anchoredPosition = subPagesTexts[_selected].anchoredPosition;
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
                _selected = (int)Mathf.Repeat(_selected, subPagesTexts.Length);
                _selectionTween?.Kill(true);
                _selectionTween = selectionBox.DOAnchorPos(subPagesTexts[_selected].anchoredPosition, duration / 2).SetEase(Ease.OutCubic).SetUpdate(true);
                
                FastSelectSubPage();
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
    }
}