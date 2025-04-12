using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Menus
{
    public class OptionsPage : Page
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