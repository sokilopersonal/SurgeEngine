using System;
using System.Collections.Generic;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters.SonicSubStates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.ActorHUD
{
    public class ActorStageHUD : MonoBehaviour
    {
        private static ActorStageHUD _instance;

        public static ActorStageHUD Context => _instance;

        [SerializeField] private RingHUD ringHUDPrefab;
        
        [Header("Ring Counter")]
        public Image ringCounter;
        public Animator ringCounterAnimator;
        public Animator ringBumpEffect;
        public TMP_Text ringCountText;
        
        [SerializeField] private RectTransform homingImage;
        [SerializeField] private QuickTimeEventUI qteUI;
        
        [Header("Speedometer")]
        [SerializeField] private Image speedometerFill;

        [Header("Boost Bar")] 
        [SerializeField] private Image boostBar;
        [SerializeField] private Image boostFill;
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
        }

        private void RingCount()
        {
            ringCountText.text = $"{Stage.Instance.data.ringCount:000}";
        }

        private void BoostBar()
        {
            FBoost boost = _actor.stateMachine.GetSubState<FBoost>();
            boostFill.fillAmount = boost.BoostEnergy / boost.maxBoostEnergy;            
            
            float boostBarWidth = Mathf.Lerp(minBoostBarSize.width, maxBoostBarSize.width, boost.maxBoostEnergy / 100);
            boostBar.rectTransform.sizeDelta = new Vector2(boostBarWidth, 50);
        }

        private void Speedometer()
        {
            float speed = _actor.rigidbody.GetHorizontalMagnitude();
            float topSpeed = _actor.stats.moveParameters.topSpeed;
            speedometerFill.fillAmount = speed / (topSpeed + topSpeed * 0.2f);
        }

        private void HomingTarget()
        {
            if (_actor.stats.homingTarget != null)
            {
                homingImage.gameObject.SetActive(true);
                var cam = _actor.camera.GetCamera();
                Vector3 position = cam.WorldToScreenPoint(_actor.stats.homingTarget.position);
                homingImage.position = position;
            }
            else
            {
                homingImage.gameObject.SetActive(false);
            }
        }

        private void OnRingCollected(ContactBase obj)
        {
            if (obj is Ring)
            {
                RingHUD ringHUDInstance = Instantiate(ringHUDPrefab, obj.transform.position, obj.transform.rotation);
                ringHUDInstance.Initialize(0.5f);
            }
        }
        
        public QuickTimeEventUI GetQTEUI()
        {
            return qteUI;
        }
    }

    [Serializable]
    public struct BoostBarSize
    {
        public float width;
    }
}