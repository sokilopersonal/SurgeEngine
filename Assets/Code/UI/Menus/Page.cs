using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Menus
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Page : MonoBehaviour
    {
        protected CanvasGroup Group { get; private set; }
        [SerializeField] private Selectable selected;
        [SerializeField] protected float duration = 0.3f;

        protected Sequence AnimationSequence { get; private set; }
        
        private void Awake()
        {
            Group = GetComponent<CanvasGroup>();
            
            Group.interactable = false;
            Group.blocksRaycasts = false;
            Group.alpha = 0;
        }

        public async UniTask Open()
        {
            var current = EventSystem.current;
            if (selected != null) current.SetSelectedGameObject(selected.gameObject);

            Group.interactable = true;
            Group.blocksRaycasts = true;
            
            AnimationSequence?.Kill(true);
            AnimationSequence = DOTween.Sequence();
            AnimationSequence.SetUpdate(true);
            InsertIntroAnimations();
            
            await AnimationSequence.AsyncWaitForCompletion();
        }
        
        public async UniTask Close()
        {
            Group.interactable = false;
            Group.blocksRaycasts = false;
            
            AnimationSequence?.Kill(true);
            AnimationSequence = DOTween.Sequence();
            AnimationSequence.SetUpdate(true);
            InsertOutroAnimations();
            
            await AnimationSequence.AsyncWaitForCompletion();
        }

        protected abstract void InsertIntroAnimations();
        protected abstract void InsertOutroAnimations();
    }
}