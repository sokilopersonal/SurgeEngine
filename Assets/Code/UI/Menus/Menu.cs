using System.Threading.Tasks;
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

        public async virtual Task Open()
        {
            EventSystem.current.SetSelectedGameObject(selected.gameObject);    
        }
        
        public async virtual Task Close()
        {
            
        }
    }
}