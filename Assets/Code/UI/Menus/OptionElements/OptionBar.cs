using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Menus.OptionElements
{
    public class OptionBar : Selectable
    {
        public event Action<MoveDirection> OnBarMove;
        public event Action<OptionBar> OnBarSelected;
        public event Action<int> OnIndexChanged; 

        [Header("States")]
        [SerializeField] private string[] states;
        [SerializeField] private TMP_Text stateText;
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

        public int Index { get; protected set; }
        private bool IsSelected
        {
            get
            {
                if (EventSystem.current == null)
                {
                    return false;
                }
                
                return EventSystem.current.currentSelectedGameObject == gameObject;
            }
        }

        public string OptionName => optionName;
        public string OptionDescription => optionDescription;

        protected override void Awake()
        {
            base.Awake();
            
            _rectTransform = GetComponent<RectTransform>();
            _autoScroll = GetComponentInParent<AutoScroll>();

            selectionGroup.alpha = 0f;
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

        private void Update()
        {
            
        }

        private void AddIndexByMove(MoveDirection obj)
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
            
            Index = Mathf.Clamp(Index, 0, states.Length - 1);
            OnIndexChanged?.Invoke(Index);
            
            UpdateText(Index);
        }

        public void SetIndex(int index)
        {
            Index = index;
            OnIndexChanged?.Invoke(index);
            
            UpdateText(Index);
        }

        public void AddIndex()
        {
            AddIndexByMove(MoveDirection.Right);
        }
        
        public void RemoveIndex()
        {
            AddIndexByMove(MoveDirection.Left);
        }

        public void UpdateText(int index)
        {
            stateText.text = states[index];

            if (name == "Master Volume")
            {
                Debug.Log(states[index]);
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
    }
}