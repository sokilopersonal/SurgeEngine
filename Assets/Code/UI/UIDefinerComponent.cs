using UnityEngine;
using UnityEngine.EventSystems;

namespace SurgeEngine.Code.UI
{
    public class UIDefinerComponent : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public Menu menuType;
        
        [SerializeField] private GameObject firstSelectedObject;

        public void Open()
        {
            PlayerUI.Instance.OpenMenu(menuType);
            if (firstSelectedObject) EventSystem.current.SetSelectedGameObject(firstSelectedObject);
        }
    }
}