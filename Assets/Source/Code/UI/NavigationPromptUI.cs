using DG.Tweening;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SurgeEngine.Source.Code.UI
{
    public class NavigationPromptUI : MonoBehaviour, IPointMarkerLoader
    {
        [SerializeField] private Image buttonImage;

        [SerializeField] private Transform center;
        [SerializeField] private Transform left;
        [SerializeField] private Transform right;

        [SerializeField] private Animator effectAnimator;

        private CharacterBase _character;
        private Camera _camera;
        private NavigationPrompt _prompt;
        private float _time;

        [Inject]
        private void InitializeCamera(CharacterBase character)
        {
            _character = character;
            _camera = character.Camera.GetCamera();
        }

        private void OnEnable()
        {
            ObjectEvents.OnButtonPromptTriggered += OnPrompt;
            _character.Input.OnButtonPressed += OnButtonPressed;
        }
        
        private void OnDisable()
        {
            ObjectEvents.OnButtonPromptTriggered -= OnPrompt;
            _character.Input.OnButtonPressed -= OnButtonPressed;
        }

        private void OnButtonPressed(ButtonType type)
        {
            if (_prompt == null || type != _prompt.ButtonType || _time <= 0f || effectAnimator.gameObject.activeSelf)
                return;

            _time = 0.15f;
            effectAnimator.gameObject.SetActive(true);
            effectAnimator.Play("Click", 0, 0);
        }
        
        private void OnPrompt(NavigationPrompt navigationPrompt)
        {
            effectAnimator.gameObject.SetActive(false);

            _prompt = navigationPrompt;
            _time = _prompt.ActiveTime;

            switch (_prompt.ButtonType)
            {
                case ButtonType.LB:
                    buttonImage.transform.SetParent(left);
                    break;
                case ButtonType.RB:
                    buttonImage.transform.SetParent(right);
                    break;
                default:
                    buttonImage.transform.SetParent(center);
                    break;
            }

            buttonImage.enabled = true;

            buttonImage.sprite = _prompt.GetSprite();
            buttonImage.transform.localPosition = Vector3.zero;

            buttonImage.color = new Color(1f, 1f, 1f, 0f);
            buttonImage.DOFade(1f, 0.1f);

            buttonImage.transform.localScale = Vector3.one * 1.25f;
            buttonImage.transform.DOScale(Vector3.one, 0.1f);
        }

        private void Hide()
        {
            _time = 0f;
            buttonImage.DOFade(0f, 0.1f);
            effectAnimator.gameObject.SetActive(false);
            _prompt = null;
        }

        private void Update()
        {
            if (_prompt == null)
                return;

            if (_time > 0f)
            {
                _time -= Time.deltaTime;

                if (_time < 0f)
                {
                    Hide();
                    return;
                }
            }

            Transform track = _prompt.TrackTransform;
            if (track != null)
            {
                Vector3 pos = _camera.WorldToScreenPoint(track.position);
                bool onScreen = _camera.IsObjectInView(track); // Make sure the point is on screen

                buttonImage.enabled = onScreen;

                if (onScreen)
                    buttonImage.transform.position = pos;
            }
        }

        public void Load()
        {
            Hide();
        }
    }
}
