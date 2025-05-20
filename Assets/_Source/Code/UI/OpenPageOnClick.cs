using System;
using NaughtyAttributes;
using SurgeEngine.Code.UI.Menus;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI
{
    [RequireComponent(typeof(Button))]
    public class OpenPageOnClick : MonoBehaviour
    {
        [SerializeField] private Page page;
        [SerializeField, Tooltip("If type is 'Completion', will wait for the page to close before opening a new one. " +
                                 "If type is 'Delayed', will open the page after the desired delay.")] private PageWaitType waitType = PageWaitType.Delayed;
        [SerializeField, ShowIf("waitType", PageWaitType.Delayed)] private float delay = 0.2f;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }
        
        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            var handler = MenusHandler.Instance;
            if (waitType == PageWaitType.Delayed)
            {
                handler.OpenMenu(page, delay);
            }
            else
            {
                handler.OpenMenu(page);
            }
        }
    }

    public enum PageWaitType
    {
        Completion,
        Delayed
    }
}