using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SurgeEngine.Code.UI
{
    public class DropdownItem : MonoBehaviour, ISubmitHandler, IMoveHandler, IScrollHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField, Required] private AutoScroll autoScroll;
        [SerializeField, Required] private TMP_Dropdown dropdown;

        public void OnSubmit(BaseEventData eventData)
        {
            //dropdown.OnSubmit(eventData);
        }

        public void OnMove(AxisEventData eventData)
        {
            autoScroll.ScrollTo(GetComponent<RectTransform>());
        }

        public void OnScroll(PointerEventData eventData)
        {
            autoScroll.ScrollRect.OnScroll(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            autoScroll.ScrollRect.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            autoScroll.ScrollRect.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
           autoScroll.ScrollRect.OnEndDrag(eventData);
        }
    }
}