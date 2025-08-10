using FMODUnity;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.Sound
{
    public class RingSpreadSound : CharacterSound
    {
        [SerializeField] private EventReference ringSpreadEvent;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            Character.OnRingLoss += OnRingLoss;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            Character.OnRingLoss -= OnRingLoss;
        }

        private void OnRingLoss()
        {
            RuntimeManager.PlayOneShot(ringSpreadEvent);
        }
    }
}