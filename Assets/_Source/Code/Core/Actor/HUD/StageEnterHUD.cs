using DG.Tweening;
using FMODUnity;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Core.Actor.HUD
{
    public class StageEnterHUD : MonoBehaviour
    {
        [Header("Elements")] 
        [SerializeField] private RectTransform blackBox;
        [SerializeField] private RectTransform blueBox;
        
        [SerializeField] private EventReference readyVoice;
        
        [Inject] private ActorBase _actor;

        private Sequence _sequence;

        private void Awake()
        {
            _sequence = DOTween.Sequence();
            _sequence.PrependInterval(0.3f);
            
            BoxTransition();

            RuntimeManager.PlayOneShot(readyVoice);
        }

        private void BoxTransition()
        {
            _sequence.Append(DOTween.To(() => blackBox.offsetMax, x => blackBox.offsetMax = x, new Vector2(-1920, blackBox.offsetMax.y), 2.5f)).SetEase(Ease.OutCubic);
            _sequence.Join(DOTween.To(() => blueBox.offsetMax, x => blueBox.offsetMax = x, new Vector2(-1920, blueBox.offsetMax.y), 2.5f).SetDelay(0.2f)).SetEase(Ease.OutCubic);
        }
    }
}