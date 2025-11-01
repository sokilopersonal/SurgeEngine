using UnityEngine;
using UnityEngine.EventSystems;

namespace SurgeEngine.Source.Code.UI.OptionBars
{
    public class NavigationOptionBar : OptionBar
    {
        public override void Set(int index)
        {
            Index = Mathf.Clamp(index, 0, definition.Values.Count - 1);
        }

        public void Next()
        {
            Index = (Index + 1) % definition.Values.Count;
        }

        public void Prev()
        {
            Index = (Index - 1 + definition.Values.Count) % definition.Values.Count;
        }

        public override void OnMove(AxisEventData eventData)
        {
            base.OnMove(eventData);
            
            var dir = eventData.moveDir;
            if (dir == MoveDirection.Left)
                Prev();
            else if (dir == MoveDirection.Right)
                Next();
        }
    }
}