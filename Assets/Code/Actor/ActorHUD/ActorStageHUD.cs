using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Parameters.SonicSubStates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.ActorHUD
{
    public class ActorStageHUD : MonoBehaviour
    {
        [SerializeField] private RingHUD ringHUDPrefab;
        [SerializeField] private Camera mainCamera;

        [SerializeField] private TMP_Text ringCounter;
        [SerializeField] private TMP_Text boostCounter;
        [SerializeField] private RectTransform homingImage;
        
        private Actor _actor => ActorContext.Context;

        private void OnEnable()
        {
            ActorEvents.OnRingCollected += OnRingCollected;
        }

        private void OnDisable()
        {
            ActorEvents.OnRingCollected -= OnRingCollected;
        }

        private void Update()
        {
            //ringCounter.text = ringCounter.text.Replace("{v}", )
            if (boostCounter != null) boostCounter.text = $"Boost Energy: {Mathf.RoundToInt(_actor.stateMachine.GetSubState<FBoost>().boostEnergy)}";

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

        private void OnRingCollected(Ring obj)
        {
            RingHUD ringHUDInstance = Instantiate(ringHUDPrefab, obj.transform.position + _actor.transform.forward * 0.05f, Quaternion.identity);
            ringHUDInstance.Initialize(0.85f);
        }
    }
}