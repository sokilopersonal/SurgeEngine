using System;
using SurgeEngine.Code.UI.Menus;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace SurgeEngine.Code.UI
{
    public class UIDefinerComponent : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        [FormerlySerializedAs("menuType")] public Page pageType;
        
        [SerializeField] private GameObject firstSelectedObject;

        private void OnEnable()
        {
            if (firstSelectedObject) EventSystem.current.SetSelectedGameObject(firstSelectedObject);
        }

        public void Open()
        {
            
        }
    }
}