using DG.Tweening;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using TMPro;
using UnityEngine;

namespace SurgeEngine
{
    public class HintTextbox : MonoBehaviour
    {
        [SerializeField] private GameObject hintBox;
        [SerializeField] private TextMeshProUGUI textAsset;
        [SerializeField] private float easeTime = 0.5f;
        [SerializeField] private Ease ease = Ease.OutBack;

        float _timer = 0f;
        HintRing _hint;

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
