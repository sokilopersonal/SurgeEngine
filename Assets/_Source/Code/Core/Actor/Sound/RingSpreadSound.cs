using FMODUnity;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class RingSpreadSound : ActorSound
    {
        [SerializeField] private EventReference ringSpreadEvent;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            Actor.OnRingLoss += OnRingLoss;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            Actor.OnRingLoss -= OnRingLoss;
        }

        private void OnRingLoss()
        {
            RuntimeManager.PlayOneShot(ringSpreadEvent);
        }
    }
}