using DG.Tweening;
using FMODUnity;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine
{
    public class ButtonPromptUI : MonoBehaviour
    {
        [SerializeField] private Image buttonImage;
        ButtonPrompt _prompt;
        float _time;

        private void OnEnable()
        {
            ObjectEvents.OnButtonPromptTriggered += OnPrompt;
        }
        private void OnDisable()
        {
            ObjectEvents.OnButtonPromptTriggered -= OnPrompt;
        }
        private void OnPrompt(ButtonPrompt buttonPrompt)
        {
            _prompt = buttonPrompt;
            _time = _prompt.GetActiveTime();

            buttonImage.enabled = true;

            buttonImage.sprite = _prompt.GetSprite();
            buttonImage.transform.localPosition = Vector3.zero;

            buttonImage.color = new Color(1f, 1f, 1f, 0f);
            buttonImage.DOFade(1f, 0.1f);

            buttonImage.transform.localScale = Vector3.one * 1.25f;
            buttonImage.transform.DOScale(Vector3.one, 0.1f);
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
                    buttonImage.DOFade(0f, 0.1f);
                    _prompt = null;
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
