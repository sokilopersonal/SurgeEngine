using Alchemy.Inspector;
using DG.Tweening;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using TMPro;
using UnityEngine;

namespace SurgeEngine
{
    public class HintTextbox : MonoBehaviour
    {
        [Header("Tween")]
        [SerializeField] private float easeTime = 0.5f;
        [SerializeField] private Ease ease = Ease.OutBack;

        [Header("References")]
        [SerializeField] private GameObject hintBox;
        [SerializeField] private TextMeshProUGUI textAsset;

        [FoldoutGroup("Button Prompts")]
        [SerializeField] private TMP_SpriteAsset xboxSprite;
        [FoldoutGroup("Button Prompts")]
        [SerializeField] private TMP_SpriteAsset playstationSprite;
        [FoldoutGroup("Button Prompts")]
        [SerializeField] private TMP_SpriteAsset keyboardSprite;

        float _timer = 0f;
        HintRing _hint;

        private TMP_SpriteAsset GetSpriteAsset()
        {
            switch (CharacterContext.Context.Input.GetDevice())
            {
                case GameDevice.Keyboard:
                    return keyboardSprite;
                case GameDevice.XboxController:
                    return xboxSprite;
                case GameDevice.Playstation:
                    return playstationSprite;
            }

            return null;
        }

        private void OnEnable()
        {
            ObjectEvents.OnHintTriggered += OnTriggered;
        }

        private void OnDisable()
        {
            ObjectEvents.OnHintTriggered -= OnTriggered;
        }

        private void Awake()
        {
            hintBox.SetActive(false);
        }

        public void Show()
        {
            hintBox.SetActive(true);
            _timer = _hint.GetMessageDuration();
            hintBox.transform.localScale = Vector3.up;
            hintBox.transform.DOScaleX(1f, easeTime).SetEase(ease);
            textAsset.text = _hint.GetMessage();
            textAsset.spriteAsset = GetSpriteAsset();
        }

        public void Hide()
        {
            hintBox.transform.DOScaleX(0f, easeTime).SetEase(ease);
        }

        public void OnTriggered(HintRing hintRing)
        {
            _hint = hintRing;
            Show();
        }

        // Update is called once per frame
        void Update()
        {
            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;
                
                if (_timer <= 0f)
                    Hide();
            }
        }
    }
}
