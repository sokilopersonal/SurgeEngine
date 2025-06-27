using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SurgeEngine.Code.UI.Pages.Baseline
{
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class Page : MonoBehaviour
    {
        // Fields
        [SerializeField] private GameObject firstSelectedObject;
        [SerializeField] private bool exitOnNewPush;
        
        // Properties
        public CanvasGroup CanvasGroup { get; private set; }
        public bool ExitOnNewPush => exitOnNewPush;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            CanvasGroup = GetComponent<CanvasGroup>();
            
            HideGroup();
        }

        public void Enter()
        {
            if (firstSelectedObject) EventSystem.current.SetSelectedGameObject(firstSelectedObject);
            
            CanvasGroup.alpha = 1f;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
        }

        public void Exit()
        {
            HideGroup();
        }

        private void HideGroup()
        {
            CanvasGroup.alpha = 0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }
    }
}