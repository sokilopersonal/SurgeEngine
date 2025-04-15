using DG.Tweening;
using UnityEngine;

namespace SurgeEngine.Code.UI.Menus.OptionTabs
{
    public class GraphicsTab : Tab
    {
        [SerializeField] private CanvasGroup background; // We need to hide the background when tab is active

        private float _startBackgroundAlpha;

        protected override void Awake()
        {
            base.Awake();
            
            _startBackgroundAlpha = background.alpha;
        }

        protected override void InsertIntroAnimations()
        {
            base.InsertIntroAnimations();
            
            AnimationSequence.Join(background.DOFade(0f, duration).From(_startBackgroundAlpha));
        }

        protected override void InsertOutroAnimations()
        {
            base.InsertOutroAnimations();
            
            AnimationSequence.Join(background.DOFade(_startBackgroundAlpha, duration));
        }
    }
}