using System.Collections;
using DG.Tweening;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace SurgeEngine.Source.Code.Infrastructure.Tools.Managers
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Image progressBar;
        [SerializeField] private TMP_Text progress;
        [SerializeField] private GameObject stageNameHolder;
        [SerializeField] private TMP_Text stageName;
        [SerializeField] private float transitionDuration = 0.5f;
        [SerializeField] private float minimalLoadTime = 2f;
        [SerializeField] private float fadeOutDelay = 1.25f;

        private Tween _groupTween;
        private Animator _animator;
        private bool _isLoading;
        private AsyncOperation _asyncOperation;

        private static SceneLoader Instance { get; set; }

        [Inject]
        private void Init(SceneLoader instance)
        {
            Instance = instance;
            _animator = GetComponent<Animator>();

            group.alpha = 0;
        }
        
        public static void LoadGameScene(string name, string displayName = "Stage Act 1")
        {
            if (!Instance._isLoading)
            {
                Instance._isLoading = true;
                Instance.stageNameHolder.SetActive(!string.IsNullOrEmpty(displayName));
                Instance.stageName.text = displayName;
                Instance._animator.Play("Load1", 0, 0);
                Instance.progress.text = "0%";
                Instance.progressBar.fillAmount = 0;
                Instance.StartCoroutine(LoadSceneRoutine(name));
            }
        }

        private static IEnumerator LoadSceneRoutine(string name)
        {
            Instance._groupTween?.Kill(true);
            Instance._groupTween = Instance.group.DOFade(1f, Instance.transitionDuration).SetEase(Ease.OutCubic).From(0).SetUpdate(true);
            Instance._groupTween.SetLink(Instance.gameObject);
            yield return Instance._groupTween.WaitForCompletion();
            
            Instance._asyncOperation = SceneManager.LoadSceneAsync(name);
            var asyncOperation = Instance._asyncOperation;

            if (asyncOperation != null)
            {
                asyncOperation.allowSceneActivation = false;

                float timer = 0f;
                float waitTime = Instance.minimalLoadTime + Random.Range(-0.5f, 0.2f);
                if (waitTime < 0)
                {
                    waitTime = 0;
                }
                while (asyncOperation.progress < 0.9f)
                {
                    float normalized = Mathf.Clamp01(asyncOperation.progress / 0.9f);
                    Instance.progressBar.fillAmount = normalized;
                    Instance.progress.text = Mathf.RoundToInt(normalized * 100f) + "%";

                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }
                
                Instance.progressBar.fillAmount = 1f;
                Instance.progress.text = "100%";
                
                while (timer < waitTime)
                {
                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }

                asyncOperation.allowSceneActivation = true;
            }
            
            Time.timeScale = 1;

            Instance._groupTween?.Kill(true);
            Instance._groupTween = Instance.group.DOFade(0f, Instance.transitionDuration).SetEase(Ease.OutCubic).From(1).SetDelay(Instance.fadeOutDelay).SetUpdate(true);
            Instance._groupTween.SetLink(Instance.gameObject);
            yield return Instance._groupTween.WaitForCompletion();
            
            Instance._isLoading = false;
        }
    }
}