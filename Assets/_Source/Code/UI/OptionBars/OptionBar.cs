using System;
using System.Collections.Generic;
using SurgeEngine.Code.Infrastructure.Custom.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.OptionBars
{
    [Serializable]
    public class OptionDefinition
    {
        public string DisplayName;
        public List<string> Values;
    }

    public class OptionBar : Selectable, ISubmitHandler, IScrollHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] protected OptionDefinition definition = new();
        public string DisplayName => definition.DisplayName;
        
        [SerializeField] protected TMP_Text title;
        [SerializeField] protected TMP_Text state;
        
        [SerializeField, TextArea] private string description;
        
        public string Description => description;

        private int _index;
        public int Index
        {
            get => _index;
            protected set
            {
                _index = value;
                OnChanged?.Invoke(this);
                
                SetTextState();
            }
        }
        public string CurrentValue => definition.Values.Count > 0 ? definition.Values[Index] : "Empty";

        private RectTransform _rectTransform;
        private AutoScroll _autoScroll;
        private ScrollRect _scrollRect;

        public event Action<OptionBar> OnChanged;

        protected override void Awake()
        {
            base.Awake();
            
            _rectTransform = GetComponent<RectTransform>();
            var autoScroll = GetComponentInParent<AutoScroll>();
            if (autoScroll)
            {
                _autoScroll = autoScroll;
                _scrollRect = _autoScroll.ScrollRect;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (title != null)
            {
                title.text = DisplayName;
            }
        }

        protected virtual void SetTextState()
        {
            state.text = CurrentValue;
        }

        public void Set(int index)
        {
            Index = index;
        }

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);
            
            var dir = eventData.moveDir;
            if (dir == MoveDirection.Up || dir == MoveDirection.Down) _autoScroll?.ScrollTo(_rectTransform);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            _autoScroll?.ScrollTo(null);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            
        }
        
        public void OnScroll(PointerEventData eventData)
        {
            if (_scrollRect != null)
            {
                _autoScroll.ScrollRect.OnScroll(eventData);
            }
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_scrollRect != null)
            {
                _autoScroll.ScrollRect.OnBeginDrag(eventData);
            }
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (_scrollRect != null)
            {
                _autoScroll.ScrollRect.OnDrag(eventData);
            }
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (_scrollRect != null)
            {
                _autoScroll.ScrollRect.OnEndDrag(eventData);
            }
        }
    }
}
