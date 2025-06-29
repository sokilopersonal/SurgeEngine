using DG.Tweening;
using UnityEngine;

namespace SurgeEngine.Code.UI.Pages.Baseline.AnimatedPages
{
    public class MainPage : Page
    {
        [SerializeField] private RectTransform container;

        protected override void Show()
        {
            base.Show();
            
            sequence.Join(container.DOAnchorPosX(25, enterDuration).From(new Vector2(-400, 0)));
        }
        
        protected override void Hide()
        {
            base.Hide();
            
            sequence.Join(container.DOAnchorPosX(-400, exitDuration));
        }
    }
}