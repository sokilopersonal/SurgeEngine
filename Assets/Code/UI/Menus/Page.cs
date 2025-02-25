using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Menus
{
    public abstract class Page : MonoBehaviour
    {
        protected CanvasGroup Group;
        [SerializeField] private Selectable selected;
        
        private void Awake() => Group = GetComponent<CanvasGroup>();

        public virtual Task Open()
        {
            EventSystem.current.SetSelectedGameObject(selected.gameObject);
            return Task.CompletedTask;
        }
        
        public virtual Task Close()
        {
            return Task.CompletedTask;
        }
    }
}