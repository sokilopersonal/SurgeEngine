using UnityEngine;
using UnityEngine.EventSystems;

namespace SurgeEngine.Code.UI.Settings
{
    public class ArrowElement : MonoBehaviour, ICancelHandler
    {
        [SerializeField] private SettingsBarElement barElement;

        public void OnCancel(BaseEventData eventData)
        {
            barElement.Select();
        }
    }
}