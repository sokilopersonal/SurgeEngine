using System;
using System.Collections.Generic;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters.SonicSubStates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SurgeEngine.Code.ActorHUD
{
    public class ActorStageHUD : MonoBehaviour
    {
        private static ActorStageHUD _instance;

        public static ActorStageHUD Context => _instance;

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
        [SerializeField] private float boostBarYAspect;
        [SerializeField] private BoostBarSize minBoostBarSize;
        [SerializeField] private BoostBarSize maxBoostBarSize;

        private Actor _actor => ActorContext.Context;

        private void Awake()
        {
            _instance = this;
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
            RingCount();
            BoostBar();
            Speedometer();
            HomingTarget();

            var stageData = Stage.Instance.data;
            float time = stageData.Time;
            timeBar.text = $"Time: {GetTimeInString(time)}";

            int score = stageData.Score;
            scoreBar.text = score > 0 ? $"{score}" : "";
        }

        private void RingCount()
        {
            ringCountText.text = $"{Stage.Instance.data.RingCount:000}";
        }

        private void BoostBar()
        {
            FBoost boost = _actor.stateMachine.GetSubState<FBoost>();
            boostFill.fillAmount = boost.BoostEnergy / boost.maxBoostEnergy;            
            
            float boostBarWidth = Mathf.Lerp(minBoostBarSize.width, maxBoostBarSize.width, boost.maxBoostEnergy / 100);
            boostBar.rectTransform.sizeDelta = new Vector2(boostBarWidth, boostBarYAspect);
        }

        private void Speedometer()
        {
            float speed = _actor.rigidbody.GetHorizontalMagnitude();
            float topSpeed = _actor.stats.moveParameters.topSpeed;
            float threshold = 2.25f;
            float roundedSpeed = Mathf.Round(speed / threshold) * threshold;
            speedometerFill.fillAmount = roundedSpeed / (topSpeed + topSpeed * 0.2f);
        }

        private void HomingTarget()
        {
            if (_actor.stats.homingTarget != null)
            {
                homingIcon.gameObject.SetActive(true);
                homingIcon.Activate();
                var cam = _actor.camera.GetCamera();
                Vector3 position = cam.WorldToScreenPoint(_actor.stats.homingTarget.position);
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
                float time = Random.Range(0.325f, 0.425f);
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