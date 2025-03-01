using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine.Code.Tools
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private GameObject screen;
        [SerializeField] private float transitionDuration = 0.5f;
        [SerializeField] private float fadeOutDelay = 1.25f;
        [SerializeField] private float artificialDelay = 2f;

        public static SceneLoader Instance { get; private set; }

        [Inject]
        private void Init(SceneLoader instance)
        {
            Instance = instance;

            group.alpha = 0;
        }
        
        public async void LoadScene(string name)
        {
            screen.SetActive(false);
            await group.DOFade(1f, transitionDuration).From(0).SetLink(gameObject).SetUpdate(true).AsyncWaitForCompletion();
            
            screen.SetActive(true);
            
            var scene = SceneManager.LoadSceneAsync(name);
            scene.allowSceneActivation = false;

            while (scene.progress < 0.9f)
                await UniTask.Yield();

            await UniTask.Delay(TimeSpan.FromSeconds(artificialDelay), DelayType.Realtime);
            
            scene.allowSceneActivation = true;

            await scene;
            await group.DOFade(0f, transitionDuration).From(1).SetDelay(fadeOutDelay).SetUpdate(true).SetLink(gameObject).AsyncWaitForCompletion();
            
            screen.SetActive(false);
        }
        
        public async void LoadScene(int index)
        {
            
        }
    }
}