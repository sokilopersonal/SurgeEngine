using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI
{
    public class AddSelectableToHandler : MonoBehaviour
    {
        private void Awake()
        {
            var current = MenuEventSystemHandler.Instance;
            var selectable = GetComponent<Selectable>();
            if (!current.Selectables.Contains(selectable))
            {
                current.Selectables.Add(selectable);
                current.AddTriggerListeners(selectable);
            }
        }
    }
}