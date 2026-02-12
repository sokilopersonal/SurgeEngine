using System.Collections;
using DG.Tweening;
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
        [SerializeField] private GameObject screen;
        [SerializeField] private Image progressBar;
        [SerializeField] private GameObject stageNameHolder;
        [SerializeField] private TMP_Text stageName;
        [SerializeField] private float transitionDuration = 0.5f;
        [SerializeField] private float fadeOutDelay = 1.25f;

        private Tween _groupTween;
        private Animator _animator;
        private bool _isLoading;

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
                Instance.StartCoroutine(LoadSceneRoutine(name));
                Instance.stageNameHolder.SetActive(!string.IsNullOrEmpty(displayName));
                Instance.stageName.text = displayName;
                Instance._animator.Play("Load1", 0, 0);
            }
        }

        private static IEnumerator LoadSceneRoutine(string name)
        {
            Instance.screen.SetActive(false);
            Instance._groupTween?.Kill(true);
            Instance._groupTween = Instance.group.DOFade(1f, Instance.transitionDuration).From(0).SetUpdate(true);
            Instance._groupTween.SetLink(Instance.gameObject);
            yield return Instance._groupTween.WaitForCompletion();
            
            Instance.screen.SetActive(true);
            
            var asyncOperation = SceneManager.LoadSceneAsync(name);
            while (asyncOperation != null && !asyncOperation.isDone)
            {
                Instance.progressBar.fillAmount = asyncOperation.progress;
                yield return null;
            }
            
            Time.timeScale = 1;

            Instance._groupTween?.Kill(true);
            Instance._groupTween = Instance.group.DOFade(0f, Instance.transitionDuration).From(1).SetDelay(Instance.fadeOutDelay).SetUpdate(true);
            Instance._groupTween.SetLink(Instance.gameObject);
            yield return Instance._groupTween.WaitForCompletion();
            
            Instance.screen.SetActive(false);
            Instance._isLoading = false;
        }
    }
}