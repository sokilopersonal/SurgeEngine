using System;
using Coffee.UIExtensions;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Gameplay.CommonObjects.Collectables;
using SurgeEngine.Code.Gameplay.CommonObjects.HUD;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SurgeEngine.Code.Core.Actor.HUD
{
    public class ActorStageHUD : MonoBehaviour
    {
        private const string TimeFormat = "<mspace=0.7em>";
        private const string ScoreFormat = "<mspace=1.0em>";

        [Header("Stage")]
        [SerializeField] private TMP_Text timeBar;
        [SerializeField] private TMP_Text scoreBar;

        [Header("Ring Counter")]
        [SerializeField] private Image ringCounter;
        [SerializeField] private Animator ringCounterAnimator;
        [SerializeField] private Animator ringBumpAnimator;
        [SerializeField] private UIParticle ringLossParticlesPrefab;
        public Image RingCounterRect => ringCounter;
        public Animator RingCounterAnimator => ringCounterAnimator;
        public Animator RingBumpAnimator => ringBumpAnimator;

        [Header("Ring HUD")]
        [SerializeField] private RingHUD ringHUDPrefab;
        [SerializeField] private TMP_Text ringCountText;
        
        [SerializeField] private HomingIcon homingIcon;

        [Header("Boost Bar")]
        [SerializeField] private BoostBarController boostBarController;

        [Inject] private ActorBase _actor;

        private void OnEnable()
        {
            ObjectEvents.OnObjectCollected += OnRingCollected;
            _actor.OnRingLoss += OnRingLoss;
        }

        private void OnDisable()
        {
            ObjectEvents.OnObjectCollected -= OnRingCollected;
            _actor.OnRingLoss -= OnRingLoss;
        }

        private void Update()
        {
            UpdateRingCount();
            boostBarController.UpdateBoostBar();
            UpdateHomingTarget();
            UpdateHUDText(Stage.Instance.data);
        }

        private void UpdateRingCount()
        {
            ringCountText.text = $"{Stage.Instance.data.RingCount:000}";
        }

        private void UpdateHUDText(StageData stageData)
        {
            timeBar.text = $"{TimeFormat}{GetTimeInString(stageData.Time)}";
            scoreBar.text = $"{ScoreFormat}{stageData.Score:000000}";
        }

        private void UpdateHomingTarget()
        {
            HomingTarget target = _actor.Kinematics.HomingTarget;
            if (target)
            {
                homingIcon.gameObject.SetActive(true);
                homingIcon.Activate();
                homingIcon.transform.position = _actor.Camera.GetCamera().WorldToScreenPoint(target.transform.position);
            }
            else
            {
                homingIcon.gameObject.SetActive(false);
            }
        }

        private void OnRingCollected(ContactBase obj)
        {
            if (obj is Ring)
            {
                RingHUD ringHUDInstance = Instantiate(ringHUDPrefab, obj.transform.position, obj.transform.rotation);
                ringHUDInstance.Initialize(this);
            }
        }

        private void OnRingLoss()
        {
            var instance = Instantiate(ringLossParticlesPrefab, ringCounter.transform);
            Destroy(instance.gameObject, 2);
        }
        
        private static string GetTimeInString(float time)
        {
            int milliseconds = Mathf.FloorToInt(time * 100f) % 100;
            int seconds = Mathf.FloorToInt(time % 60);
            int minutes = Mathf.FloorToInt(time / 60);
            return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
        }
    }
}