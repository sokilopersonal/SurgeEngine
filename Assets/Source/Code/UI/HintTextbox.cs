using System.Collections;
using Alchemy.Inspector;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using TMPro;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.UI
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

        [FoldoutGroup("Sound")]
        [SerializeField] private EventReference letterSound;

        private float _timer;
        private HintRing _hint;
        private EventInstance _letterSoundInstance;
        
        [Inject] private CharacterBase _character;

        private void Awake()
        {
            hintBox.SetActive(false);
            _letterSoundInstance = RuntimeManager.CreateInstance(letterSound);
        }

        private void OnEnable()
        {
            ObjectEvents.OnHintTriggered += OnTriggered;
        }

        private void OnDisable()
        {
            ObjectEvents.OnHintTriggered -= OnTriggered;
        }

        private void Update()
        {
            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;

                if (_timer <= 0f)
                    Hide();
            }
        }

        private void Show()
        {
            StopCoroutine(Typewriter());

            hintBox.SetActive(true);
            _timer = _hint.CurrentMessage.messageDuration;
            hintBox.transform.localScale = Vector3.up;
            hintBox.transform.DOScaleX(1f, easeTime).SetEase(ease);

            textAsset.text = _hint.CurrentMessage.message;

            textAsset.spriteAsset = GetSpriteAsset();

            if (_hint.CurrentMessage.animationDuration > 0f)
                StartCoroutine(Typewriter());
            else
                ResetTextAlpha();
        }

        private void SetCharAlpha(TMP_TextInfo textInfo, int iCharId, int iAlpha)
        {
            if (iCharId >= textInfo.characterInfo.Length)
                return;
            
            int iMaterialIndex = textInfo.characterInfo[iCharId].materialReferenceIndex;
            Color32[] rVertexColors = textInfo.meshInfo[iMaterialIndex].colors32;
            int iVertexIndex = textInfo.characterInfo[iCharId].vertexIndex;

            byte alpha = (byte)Mathf.Clamp(iAlpha, 0, 255);

            rVertexColors[iVertexIndex + 0].a = alpha;
            rVertexColors[iVertexIndex + 1].a = alpha;
            rVertexColors[iVertexIndex + 2].a = alpha;
            rVertexColors[iVertexIndex + 3].a = alpha;
        }

        private void ResetTextAlpha(int alpha = 255)
        {
            textAsset.ForceMeshUpdate();

            TMP_TextInfo info = textAsset.textInfo;

            for (int i = 0; i < info.characterCount; i++)
            {
                if (!info.characterInfo[i].isVisible)
                    continue;

                SetCharAlpha(info, i, alpha);
                textAsset.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            }
        }

        private IEnumerator Typewriter()
        {
            ResetTextAlpha(0);
            TMP_TextInfo info = textAsset.textInfo;
            int charCount = 0;
            
            for (int i = 0; i < info.characterCount; i++)
            {
                if (!info.characterInfo[i].isVisible)
                    continue;

                charCount++;
            }

            float textTime = _hint.CurrentMessage.animationDuration / charCount;
            
            for (int i = 0; i < info.characterCount; i++)
            {
                if (!info.characterInfo[i].isVisible)
                    continue;

                int alpha = 0;

                DOTween.To(() => alpha, x => alpha = x, 255, textTime).OnUpdate(() =>
                {
                    SetCharAlpha(info, i, alpha);
                    textAsset.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
                });

                _letterSoundInstance.start();

                yield return new WaitForSeconds(textTime);
            }
        }

        public void Hide()
        {
            hintBox.transform.DOScaleX(0f, easeTime).SetEase(ease);
        }

        private void OnTriggered(HintRing hintRing)
        {
            _hint = hintRing;
            Show();
        }

        private TMP_SpriteAsset GetSpriteAsset()
        {
            switch (_character.Input.Device)
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
    }
}
