using UnityEngine;
using UnityEngine.EventSystems;

namespace SurgeEngine.Code.UI
{
    public class UnselectOnHover : MonoBehaviour, IPointerEnterHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}