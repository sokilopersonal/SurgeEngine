using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI
{
    public abstract class Menu : MonoBehaviour
    {
        protected CanvasGroup Group;
        [SerializeField] private Selectable selected;
        
        private void Awake() => Group = GetComponent<CanvasGroup>();

        public virtual void Open()
        {
            EventSystem.current.SetSelectedGameObject(selected.gameObject);    
        }
        
        public virtual void Close()
        {
            
        }
    }
}