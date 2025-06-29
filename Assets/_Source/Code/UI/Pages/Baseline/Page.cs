using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SurgeEngine.Code.UI.Pages.Baseline
{
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class Page : MonoBehaviour
    {
        [Header("Transition")]
        [SerializeField] private GameObject firstSelectedObject;
        [SerializeField] private bool exitOnNewPush;
        [SerializeField, Tooltip("Animated enter duration.")] protected float enterDuration = 0.5f;
        [SerializeField, Tooltip("Animated exit duration.")] protected float exitDuration = 0.5f;
        
        public CanvasGroup CanvasGroup { get; private set; }
        public bool ExitOnNewPush => exitOnNewPush;
        
        public UnityEvent OnPreEnter;
        public UnityEvent OnPostEnter; 
        public UnityEvent OnPreExit;
        public UnityEvent OnPostExit;
        
        protected Sequence sequence;

        protected virtual void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            
            CanvasGroup.alpha = 0f;
            Block(true);
        }

        public void Enter()
        {
            OnPreEnter.Invoke();
            
            if (firstSelectedObject) EventSystem.current.SetSelectedGameObject(firstSelectedObject);
            
            Block(false);
            Show();
        }

        public void Exit()
        {
            OnPreExit.Invoke();
            
            Block(true);
            Hide();
        }

        protected virtual void Show()
        {
            CreateSequence();
            sequence.onComplete += () => OnPostEnter.Invoke();
            
            sequence.Join(CanvasGroup.DOFade(1f, enterDuration));
        }

        protected virtual void Hide()
        {
            CreateSequence();
            sequence.onComplete += () => OnPostExit.Invoke();
            
            sequence.Join(CanvasGroup.DOFade(0f, exitDuration));
        }

        private void CreateSequence()
        {
            sequence?.Kill(true);
            sequence = DOTween.Sequence();
            sequence.SetUpdate(true);
        }

        public void Block(bool value)
        {
            CanvasGroup.interactable = !value;
            CanvasGroup.blocksRaycasts = !value;
        }
    }
}