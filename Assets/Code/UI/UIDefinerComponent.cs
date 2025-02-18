using UnityEngine;

namespace SurgeEngine.Code.UI
{
    public class UIDefinerComponent : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public Menu menuType;

        public void Open()
        {
            PlayerUI.Instance.OpenMenu(menuType);
        }
    }
}