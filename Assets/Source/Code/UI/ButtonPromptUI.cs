using DG.Tweening;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine
{
    public class ButtonPromptUI : MonoBehaviour
    {
        [SerializeField] private Image buttonImage;

        [SerializeField] private Transform center;
        [SerializeField] private Transform left;
        [SerializeField] private Transform right;

        [SerializeField] private Animator effectAnimator;

        ButtonPrompt _prompt;
        float _time;

        private void OnEnable()
        {
            ObjectEvents.OnButtonPromptTriggered += OnPrompt;
            CharacterContext.Context.Input.OnButtonPressed += OnButtonPressed;
        }
        private void OnDisable()
        {
            ObjectEvents.OnButtonPromptTriggered -= OnPrompt;
            CharacterContext.Context.Input.OnButtonPressed -= OnButtonPressed;
        }

        private void OnButtonPressed(ButtonType type)
        {
            if (_prompt == null || type != _prompt.GetButtonType() || _time <= 0f || effectAnimator.gameObject.activeSelf)
                return;

            _time = .15f;
            effectAnimator.gameObject.SetActive(true);
            effectAnimator.Play("Click", 0, 0);
        }
        private void OnPrompt(ButtonPrompt buttonPrompt)
        {
            effectAnimator.gameObject.SetActive(false);

            _prompt = buttonPrompt;
            _time = _prompt.GetActiveTime();

            switch (_prompt.GetButtonType())
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

            Transform track = _prompt.GetTransform();

            if (track != null)
            {
                Vector3 pos = Camera.main.WorldToScreenPoint(track.position);

                bool onScreen = pos.z >= 0f && (pos.x >= Screen.safeArea.xMin && pos.x <= Screen.safeArea.xMax) && (pos.y >= Screen.safeArea.yMin && pos.y <= Screen.safeArea.yMax); // Make sure point is on screen

                buttonImage.enabled = onScreen;

                if (onScreen)
                    buttonImage.transform.position = pos;
            }
        }
    }
}
