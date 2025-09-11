using System;
using System.Collections;
using DG.Tweening;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine._Source.Code.Infrastructure.Tools.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine._Source.Code.UI
{
    public class PointMarkerLoadingScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Animator animator;

        [Inject] private Stage _stage;
        [Inject] private CharacterBase _character;
        
        private Tween _tween;
        private Coroutine _loadingCoroutine;

        private void Awake()
        {
            group.alpha = 0f;
        }

        private void OnEnable()
        {
            _character.Life.OnDied += OnCharacterDied;
        }

        private void OnDisable()
        {
            _character.Life.OnDied -= OnCharacterDied;
        }

        private void OnCharacterDied(CharacterBase obj)
        {
            if (_loadingCoroutine != null)
                StopCoroutine(_loadingCoroutine);
            _loadingCoroutine = StartCoroutine(LoadCurrentPointMarker());
        }

        private IEnumerator LoadCurrentPointMarker()
        {
            if (_stage.CurrentPointMarker != null)
            {
                yield return new WaitForSeconds(1.5f);
                yield return Play();
                _stage.CurrentPointMarker.Load();
                _stage.Data.Score = 0;
                _stage.Data.RingCount = 0;
                yield return Hide();
            }
            else
            {
                yield return new WaitForSeconds(1.25f);
                SceneLoader.LoadGameScene(SceneManager.GetActiveScene().name);
            }
        }

        public IEnumerator Play()
        {
            animator.Play("Intro", 0, 0f);
            yield return group.DOFade(1f, 1f).From(0).WaitForCompletion();
            yield return new WaitForSecondsRealtime(1.25f);
        }

        public IEnumerator Hide()
        {
            yield return group.DOFade(0f, 0.25f).WaitForCompletion();
        }
    }
}