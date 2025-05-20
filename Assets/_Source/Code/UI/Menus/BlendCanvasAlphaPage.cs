using DG.Tweening;
using UnityEngine;

namespace SurgeEngine.Code.UI.Menus
{
    public class BlendCanvasAlphaPage : Page
    {
        [SerializeField] private Ease easing;

        protected override void InsertIntroAnimations()
        {
            AnimationSequence.Join(Group.DOFade(1f, duration).From(0)).SetEase(easing);
        }

        protected override void InsertOutroAnimations()
        {
            AnimationSequence.Join(Group.DOFade(0f, duration)).SetEase(easing);
        }
    }
}