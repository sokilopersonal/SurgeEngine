using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using UnityEngine;

namespace SurgeEngine.Code.ActorHUD
{
    public class ActorStageHUD : MonoBehaviour
    {
        [SerializeField] private RingHUD ringHUDPrefab;
        [SerializeField] private Camera mainCamera;
        
        private Actor _actor => ActorContext.Context;

        private void OnEnable()
        {
            ActorEvents.OnRingCollected += OnRingCollected;
        }

        private void OnDisable()
        {
            ActorEvents.OnRingCollected -= OnRingCollected;
        }

        private void OnRingCollected(Ring obj)
        {
            return;
            RingHUD ringHUDInstance = Instantiate(ringHUDPrefab, obj.transform.position, Quaternion.identity);
            ringHUDInstance.Initialize(1f);
        }
    }
}