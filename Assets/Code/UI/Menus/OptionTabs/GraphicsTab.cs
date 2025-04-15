using DG.Tweening;
using UnityEngine;

namespace SurgeEngine.Code.UI.Menus.OptionTabs
{
    public class GraphicsTab : Page
    {
        protected override void InsertIntroAnimations()
        {
            AnimationSequence.Join(Group.DOFade(1f, duration).From(0));
        }

        protected override void InsertOutroAnimations()
        {
            AnimationSequence.Join(Group.DOFade(0f, duration));
        }
    }
}