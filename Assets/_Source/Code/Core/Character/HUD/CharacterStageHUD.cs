using Coffee.UIExtensions;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Core.Character.System.Characters.Sonic;
using SurgeEngine._Source.Code.Gameplay.CommonObjects;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.Collectables;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.GoalRing;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.HUD;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SurgeEngine._Source.Code.Core.Character.HUD
{
    public class CharacterStageHUD : MonoBehaviour
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

        [Inject] private CharacterBase _character;
        [Inject] private DiContainer _instantiator;
        
        private Camera _camera;

        private void Awake()
        {
            _camera = _character.Camera.GetCamera();
        }

        private void OnEnable()
        {
            ObjectEvents.OnObjectTriggered += OnObjectTriggered;
            _character.Life.OnRingLoss += OnRingLoss;
        }

        private void OnDisable()
        {
            ObjectEvents.OnObjectTriggered -= OnObjectTriggered;
            _character.Life.OnRingLoss -= OnRingLoss;
        }

        private void Update()
        {
            UpdateRingCount();
            boostBarController.UpdateBoostBar();
            UpdateHomingTarget();
            UpdateHUDText(Stage.Instance.Data);
        }

        private void UpdateRingCount()
        {
            ringCountText.text = $"{Stage.Instance.Data.RingCount:000}";
        }

        private void UpdateHUDText(StageData stageData)
        {
            timeBar.text = $"{TimeFormat}{GetTimeInString(stageData.Time)}";
            scoreBar.text = $"{ScoreFormat}{stageData.Score:000000}";
        }

        private void UpdateHomingTarget()
        {
            HomingTarget target = (_character.Kinematics as SonicKinematics)?.HomingTarget;
            if (target)
            {
                if (_camera.IsObjectInView(target.transform)) homingIcon.gameObject.SetActive(true);
                homingIcon.Activate();
                homingIcon.transform.position = _camera.WorldToScreenPoint(target.transform.position);
            }
            else
            {
                homingIcon.gameObject.SetActive(false);
            }
        }

        private void OnObjectTriggered(ContactBase obj)
        {
            if (obj is Ring ring)
            {
                if (!ring.IsSuperRing())
                {
                    RingHUD ringHUDInstance = Instantiate(ringHUDPrefab, obj.transform.position, obj.transform.rotation);
                    _instantiator.InjectGameObject(ringHUDInstance.gameObject);
                    ringHUDInstance.Initialize(this);
                }
            }

            if (obj is GoalRing goalRing)
            {
                var screen = goalRing.CurrentGoalScreen;
                screen.OnFlashEnd += () => gameObject.SetActive(false);
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