using FMODUnity;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.HUD
{
    public class HomingIcon : MonoBehaviour
    {
        [SerializeField] private new Animation animation;
        [SerializeField] private EventReference soundReference;

        private bool _activated;

        public void Activate()
        {
            if (!_activated)
            {
                animation.Play(PlayMode.StopSameLayer);
                RuntimeManager.PlayOneShot(soundReference);
                _activated = true;
            }
        }

        private void OnDisable()
        {
            _activated = false;
        }
    }
}