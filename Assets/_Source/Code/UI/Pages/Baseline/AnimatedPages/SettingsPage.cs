using DG.Tweening;
using SurgeEngine.Code.Infrastructure.Tools.Managers.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Pages.Baseline.AnimatedPages
{
    public class SettingsPage : Page
    {
        [SerializeField] private Page[] subPages;
        private Page _current;
        private OptionUI _currentOptionUI;
        
        [SerializeField] private Image background;

        protected override void Show()
        {
            base.Show();
            
            sequence.Join(background.DOFade(0.2f, enterDuration));
        }
        
        protected override void Hide()
        {
            base.Hide();
            
            sequence.Join(background.DOFade(1f, exitDuration));
        }

        public void Save()
        {
            _currentOptionUI.Save();
        }
        
        public void Revert()
        {
            _currentOptionUI.Revert();
        }
        
        public void SubPush(Page page)
        {
            if (_current == page) return;
            _current = page;
            _currentOptionUI = page.GetComponent<OptionUI>();
            
            foreach (var subPage in subPages)
            {
                subPage.CanvasGroup.alpha = 0f;
                subPage.CanvasGroup.interactable = false;
                subPage.CanvasGroup.blocksRaycasts = false;
            }
            
            page.CanvasGroup.DOFade(1f, 0.5f).SetUpdate(true);
            page.CanvasGroup.interactable = true;
            page.CanvasGroup.blocksRaycasts = true;
            _current = page;
        }
    }
}