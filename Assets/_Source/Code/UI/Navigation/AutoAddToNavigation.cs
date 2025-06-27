using SurgeEngine.Code.Infrastructure.Custom.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Navigation
{
    /// <summary>
    /// A script to automatically add a selectable instance this object has to MenuNavigationSystem.
    /// </summary>
    public class AutoAddToNavigation : MonoBehaviour
    {
        private void Awake()
        {
            var system = GetComponentInParent<MenuNavigationSystem>();
            system.Add(GetComponent<Selectable>());
        }
    }
}