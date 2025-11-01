using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.Tools.Managers
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private GameObject screen;
        [SerializeField] private float transitionDuration = 0.5f;
        [SerializeField] private float fadeOutDelay = 1.25f;

        private Tween _groupTween;
        private bool _isLoading;

        private static SceneLoader Instance { get; set; }

        [Inject]
        private void Init(SceneLoader instance)
        {
            Instance = instance;

            group.alpha = 0;
        }
        
        public static void LoadGameScene(string name)
        {
            if (!Instance._isLoading)
            {
                Instance.StartCoroutine(LoadSceneRoutine(name));
            }
        }

        private static IEnumerator LoadSceneRoutine(string name)
        {
            Instance._isLoading = true;
            Instance.screen.SetActive(false);
            Instance._groupTween?.Kill(true);
            Instance._groupTween = Instance.group.DOFade(1f, Instance.transitionDuration).From(0).SetUpdate(true);
            Instance._groupTween.SetLink(Instance.gameObject);
            yield return Instance._groupTween.WaitForCompletion();
            
            Instance.screen.SetActive(true);
            
            var asyncOperation = SceneManager.LoadSceneAsync(name);
            while (asyncOperation != null && !asyncOperation.isDone)
            {
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