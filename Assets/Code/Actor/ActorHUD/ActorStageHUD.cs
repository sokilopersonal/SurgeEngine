using System;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.ActorHUD
{
    public class ActorStageHUD : MonoBehaviour
    {
        [SerializeField] private RingHUD ringHUDPrefab;

        [SerializeField] private TMP_Text timeBar;
        [SerializeField] private TMP_Text scoreBar;
        
        [Header("Ring Counter")]
        public Image ringCounter;
        public Animator ringCounterAnimator;
        public Animator ringBumpEffect;
        public TMP_Text ringCountText;
        
        [SerializeField] private HomingIcon homingIcon;
        [SerializeField] private QuickTimeEventUI qteUI;
        
        [Header("Speedometer")]
        [SerializeField] private Image speedometerFill;

        [Header("Boost Bar")] 
        [SerializeField] private Image boostBar;
        [SerializeField] private Image boostFill;
        [SerializeField] private Image boostFill2;
        [SerializeField, Range(0, 100)] private float energy; 
        [SerializeField] private float energyDivider; 
        [SerializeField] private float boostBarYAspect;
        [SerializeField] private BoostBarSize minBoostBarSize;
        [SerializeField] private BoostBarSize maxBoostBarSize;
        private float _extEnergy;

        private Actor _actor;
        
        private void OnValidate()
        {
            float energy = this.energy / 100;
            boostFill.materialForRendering.SetFloat("_BoostAmount", energy);
            boostFill.materialForRendering.SetFloat("_SplitAmount", energyDivider);
            boostFill2.fillAmount = energy + 0.01f;

            if (Mathf.Approximately(energy, 0f))
            {
                boostFill2.fillAmount = 0;
            }
        }

        private void OnEnable()
        {
            ObjectEvents.OnObjectCollected += OnRingCollected;
        }

        private void OnDisable()
        {
            ObjectEvents.OnObjectCollected -= OnRingCollected;
        }

        private void Update()
        {
            _actor = ActorContext.Context;
            
            RingCount();
            BoostBar();
            Speedometer();
            HomingTarget();

            var stageData = Stage.Instance.data;
            float time = stageData.Time;

            string mono = "<mspace=0.7em>";
            string scoreMono = "<mspace=1.0em>";
            
            timeBar.text = $"{mono}{GetTimeInString(time)}";

            int score = stageData.Score;
            scoreBar.text = $"{scoreMono}{score:000000}";
        }

        private void RingCount()
        {
            ringCountText.text = $"{Stage.Instance.data.RingCount:000}";
        }

        private void BoostBar()
        {
            FBoost boost = _actor.stateMachine.GetSubState<FBoost>();
            
            float energy = boost.BoostEnergy / 100;
            boostFill.materialForRendering.SetFloat("_BoostAmount", energy);
            boostFill.materialForRendering.SetFloat("_SplitAmount", energyDivider);
            boostFill2.fillAmount = energy + 0.01f;

            if (Mathf.Approximately(energy, 0f))
            {
                boostFill2.fillAmount = 0;
            }
            
            float boostBarWidth = Mathf.Lerp(minBoostBarSize.width, maxBoostBarSize.width, 100 / 100);
            boostBar.rectTransform.sizeDelta = new Vector2(boostBarWidth, boostBarYAspect);
        }

        private void Speedometer()
        {
            // float speed = _actor.kinematics.HorizontalSpeed;
            // float topSpeed = _actor.stats.moveParameters.topSpeed;
            // float threshold = 2.25f;
            // float roundedSpeed = Mathf.Round(speed / threshold) * threshold;
            // speedometerFill.fillAmount = roundedSpeed / (topSpeed + topSpeed * 0.2f);
        }

        private void HomingTarget()
        {
            var target = _actor.stats.homingTarget;
            if (target)
            {
                homingIcon.gameObject.SetActive(true);
                homingIcon.Activate();
                var cam = _actor.camera.GetCamera();
                Vector3 position = cam.WorldToScreenPoint(target.transform.position);
                homingIcon.transform.position = position;
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
                float time = 0.425f;
                ringHUDInstance.Initialize(time);
            }
        }
        
        public QuickTimeEventUI GetQTEUI()
        {
            return qteUI;
        }
        
        private static string GetTimeInString(float time)
        {
            int milliseconds = Mathf.FloorToInt(time * 100f) % 100;
            int seconds = Mathf.FloorToInt(time % 60);
            int minutes = Mathf.FloorToInt(time / 60);
            return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
        }
    }

    [Serializable]
    public struct BoostBarSize
    {
        public float width;
    }
}