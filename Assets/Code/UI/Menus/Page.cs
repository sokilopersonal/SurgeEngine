using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Menus
{
    public abstract class Page : MonoBehaviour
    {
        protected CanvasGroup Group;
        [SerializeField] private Selectable selected;

        protected Sequence AnimationSequence { get; private set; }
        
        private void Awake()
        {
            Group = GetComponent<CanvasGroup>();
        }

        public async UniTask Open()
        {
            EventSystem.current.SetSelectedGameObject(selected.gameObject);
            AnimationSequence?.Kill(true);
            AnimationSequence = DOTween.Sequence();
            AnimationSequence.SetUpdate(true);
            InsertIntroAnimations();
            
            await AnimationSequence.AsyncWaitForCompletion();
        }
        
        public async UniTask Close()
        {
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