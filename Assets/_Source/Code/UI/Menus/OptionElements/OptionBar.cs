using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Menus.OptionElements
{
    public class OptionBar : Selectable, IScrollHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event Action<MoveDirection> OnBarMove;
        public event Action<OptionBar> OnBarSelected;
        public event Action<int> OnIndexChanged; 

        [Header("States")]
        [SerializeField] private string[] states;
        [SerializeField] protected TMP_Text stateText;
        public string[] States => states;

        [Header("Start Index")] 
        [SerializeField, Min(0)] private int startIndex;

        [Header("Description")] 
        [SerializeField] private string optionName;
        [SerializeField, TextArea] private string optionDescription;
        
        [Header("Selection")]
        [SerializeField] private CanvasGroup selectionGroup;
        private Tween _selectionTween;
        private RectTransform _rectTransform;
        private AutoScroll _autoScroll;
        
        private ScrollRect _parentScrollRect;

        public int Index
        {
            get => _index;
            protected set
            {
                if (states.Length > 0)
                {
                    _index = Mathf.Clamp(value, 0, states.Length - 1);
                }
                else
                {
                    _index = value;
                }
                OnIndexChanged?.Invoke(_index);
            
                UpdateText(_index);
            }
        }
        private int _index;
        
        private bool IsSelected
        {
            get
            {
                var current = EventSystem.current;
                if (current == null)
                {
                    return false;
                }
                
                return current.currentSelectedGameObject == gameObject;
            }
        }

        public string OptionName => optionName;
        public string OptionDescription => optionDescription;

        protected override void Awake()
        {
            base.Awake();

            if (Application.isPlaying)
            {
                _rectTransform = GetComponent<RectTransform>();
                _autoScroll = GetComponentInParent<AutoScroll>();

                var parentScroll = GetComponentInParent<ScrollRect>();
                if (parentScroll != null)
                {
                    _parentScrollRect = parentScroll;
                }

                selectionGroup.alpha = 0f;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (Application.isPlaying)
            {
                OnBarMove += AddIndexByMove;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (Application.isPlaying)
            {
                OnBarMove -= AddIndexByMove;
            }
        }

        protected virtual void AddIndexByMove(MoveDirection obj)
        {
            switch (obj)
            {
                case MoveDirection.Left:
                    Index--;
                    break;
                case MoveDirection.Right:
                    Index++;
                    break;
            }
        }

        public virtual void SetIndex(int index)
        {
            Index = index;
        }

        public void AddIndex()
        {
            AddIndexByMove(MoveDirection.Right);
        }
        
        public void RemoveIndex()
        {
            AddIndexByMove(MoveDirection.Left);
        }

        public virtual void UpdateText(int index)
        {
            if (states.Length > 0 && index >= 0 && index < states.Length)
            {
                stateText.text = states[index];
            }
        }

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);
            
            _autoScroll.ScrollTo(_rectTransform);
            
            if (IsSelected)
            {
                OnBarMove?.Invoke(eventData.moveDir);
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            
            OnBarSelected?.Invoke(this);
            _autoScroll.ScrollTo(null);

            _selectionTween?.Kill(true);
            _selectionTween = selectionGroup.DOFade(1f, 0.25f).SetUpdate(true).SetEase(Ease.OutCubic);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            _selectionTween?.Kill(true);
            _selectionTween = selectionGroup.DOFade(0f, 0.25f).SetUpdate(true).SetEase(Ease.OutCubic);
        }

        public void OnScroll(PointerEventData eventData)
        {
            if (_parentScrollRect != null)
            {
                _parentScrollRect.OnScroll(eventData);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_parentScrollRect != null)
            {
                _parentScrollRect.OnBeginDrag(eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_parentScrollRect != null)
            {
                _parentScrollRect.OnDrag(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_parentScrollRect != null)
            {
                _parentScrollRect.OnEndDrag(eventData);
            }
        }
    }
}