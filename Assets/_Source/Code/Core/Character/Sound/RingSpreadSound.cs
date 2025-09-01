using FMODUnity;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.Sound
{
    public class RingSpreadSound : CharacterSound
    {
        [SerializeField] private EventReference ringSpreadEvent;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            Character.Life.OnRingLoss += OnRingLoss;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            Character.Life.OnRingLoss -= OnRingLoss;
        }

        private void OnRingLoss()
        {
            RuntimeManager.PlayOneShot(ringSpreadEvent);
        }
    }
}