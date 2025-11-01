using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Source.Code.Core.Character.HUD
{
    public class ButtonAppearance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _image;

        private Tween _fillTween;

        public void OnPointerEnter(PointerEventData eventData)
        {
            _fillTween?.Kill();
            _fillTween = _image.DOFillAmount(1f, 0.5f).SetEase(Ease.OutCubic);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            _fillTween?.Kill();
            _fillTween = _image.DOFillAmount(0f, 0.25f).SetEase(Ease.InCubic);
        }
    }
}