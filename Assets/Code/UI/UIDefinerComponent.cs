using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SurgeEngine.Code.UI
{
    public class UIDefinerComponent : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public Menu menuType;
        
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