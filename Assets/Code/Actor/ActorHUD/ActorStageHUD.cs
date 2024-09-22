using System.Globalization;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Parameters.SonicSubStates;
using TMPro;
using UnityEngine;

namespace SurgeEngine.Code.ActorHUD
{
    public class ActorStageHUD : MonoBehaviour
    {
        [SerializeField] private RingHUD ringHUDPrefab;
        [SerializeField] private Camera mainCamera;

        [SerializeField] private TMP_Text ringCounter;
        [SerializeField] private TMP_Text boostCounter;
        
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
            boostCounter.text = $"Boost Energy: {Mathf.RoundToInt(_actor.stateMachine.GetSubState<FBoost>().boostEnergy)}";
        }

        private void OnRingCollected(Ring obj)
        {
            RingHUD ringHUDInstance = Instantiate(ringHUDPrefab, obj.transform.position, Quaternion.identity);
            ringHUDInstance.Initialize(0.45f);
        }
    }
}