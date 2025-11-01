using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Source.Code.UI.OptionBars
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
        [SerializeField] private AutoScroll autoScroll;
        private string DisplayName => definition.DisplayName;
        
        [SerializeField] protected TMP_Text title;
        [SerializeField] protected TMP_Text state;

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

        public event Action<OptionBar> OnChanged;

        protected override void Awake()
        {
            base.Awake();
            
            _rectTransform = GetComponent<RectTransform>();
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

        public virtual void Set(int index)
        {
            Index = index;
        }
        
        public void AddOption(string value)
        {
            definition.Values.Add(value);
        }

        public void RemoveOption(string value)
        {
            definition.Values.Remove(value);
        }
        
        public void ClearOptions()
        {
            definition.Values.Clear();
        }

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);
            
            var dir = eventData.moveDir;
            if (dir == MoveDirection.Up || dir == MoveDirection.Down) autoScroll?.ScrollTo(_rectTransform);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);

            autoScroll?.ScrollTo(null);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            
        }
        
        public void OnScroll(PointerEventData eventData)
        {
            if (autoScroll != null)
            {
                autoScroll.ScrollRect.OnScroll(eventData);
            }
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (autoScroll != null)
            {
                autoScroll.ScrollRect.OnBeginDrag(eventData);
            }
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (autoScroll != null)
            {
                autoScroll.ScrollRect.OnDrag(eventData);
            }
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            if (autoScroll != null)
            {
                autoScroll.ScrollRect.OnEndDrag(eventData);
            }
        }
    }
}
