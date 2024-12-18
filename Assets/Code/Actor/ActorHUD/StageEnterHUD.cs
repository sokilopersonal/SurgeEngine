using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorSystem;

namespace SurgeEngine.Code.ActorHUD
{
    public class StageEnterHUD : MonoBehaviour
    {
        [SerializeField] private RectTransform[] arrows;
        [SerializeField] private RectTransform ready;
        [SerializeField] private RectTransform go;
        [SerializeField] private CanvasGroup flash;

        [SerializeField] private EventReference readyVoice;
        [SerializeField] private EventReference goVoice;

        private void Start()
        {
            CanvasGroup group = GetComponent<CanvasGroup>();
            StartType startType = FindFirstObjectByType<ActorStartDefiner>().startData.startType;
            group.alpha = 0;

            if (startType == StartType.None)
            {
                Arrows(group);

                ready.GetComponent<CanvasGroup>().alpha = 0;
                go.GetComponent<CanvasGroup>().alpha = 0;
            }
            else
            {
                Arrows(group);
                Prepare();
            }
        }
        
        private void Arrows(CanvasGroup group)
        {
            group.alpha = 1;
            float pad = 1920 * 1.25f;
            float duration = 1.35f;
            flash.alpha = 0;
            for (int i = 0; i < arrows.Length; i++)
            {
                switch (i)
                {
                    case 0 or 2:
                        arrows[i]
                            .DOAnchorPosX(-pad, duration)
                            .SetEase(Ease.OutCubic)
                            .From(new Vector2(0, arrows[i].anchoredPosition.y));
                        break;
                    case 1:
                        arrows[i]
                            .DOAnchorPosX(pad, duration)
                            .SetEase(Ease.OutCubic)
                            .From(new Vector2(0, arrows[i].anchoredPosition.y));
                        break;
                }
                
                arrows[i].DOSizeDelta(new Vector2(2400f, arrows[i].sizeDelta.y), duration).SetEase(Ease.InQuad);
            }
        }
        
        private void Prepare()
        {
            RuntimeManager.PlayOneShot(readyVoice);
            ready.DOScale(1.5f, 2.75f).SetEase(Ease.Linear).onComplete = () => 
            {
                ready.GetComponent<CanvasGroup>().DOFade(0f, 0.2f);

                Sequence sequence = DOTween.Sequence();

                TweenerCore<float, float, FloatOptions> fadeTween = go.GetComponent<CanvasGroup>().DOFade(1f, 0.25f);
                sequence.Append(fadeTween);
                fadeTween.onComplete += () => RuntimeManager.PlayOneShot(goVoice);
                TweenerCore<Vector2, Vector2, VectorOptions> tween = go.DOAnchorPosX(0, 0.35f).SetEase(Ease.OutCubic).From(new Vector2(-1920f, 0));
                sequence.Append(tween);
                sequence.Append(go.DOAnchorPosX(3000f, 0.45f).SetEase(Ease.OutCubic));
                sequence.Append(go.GetComponent<CanvasGroup>().DOFade(0f, 0.25f));
                
                Sequence flashSequence = DOTween.Sequence();
                flashSequence.Append(flash.DOFade(1f, 0.075f)).SetDelay(0.25f);
                flashSequence.Append(flash.DOFade(0f, 0.25f));
            };
        }
    }
}