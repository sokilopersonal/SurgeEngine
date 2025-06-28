using DG.Tweening;

namespace SurgeEngine.Code.UI.Pages.Baseline.AnimatedPages
{
    public abstract class AnimatedPage : Page
    {
        protected Sequence sequence;

        protected override void Show()
        {
            base.Show();
            
            Intro();
        }

        protected override void Hide()
        {
            base.Hide();
            
            Outro();
        }

        protected virtual void Intro()
        {
            CreateSequence();
        }

        protected virtual void Outro()
        {
            CreateSequence();
        }

        private void CreateSequence()
        {
            sequence?.Kill(true);
            sequence = DOTween.Sequence();
            sequence.SetUpdate(true);
        }
    }
}