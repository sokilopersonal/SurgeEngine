using FMODUnity;
using UnityEngine;
using Zenject;
using VolumeManager = SurgeEngine.Source.Code.Infrastructure.Tools.Managers.VolumeManager;

namespace SurgeEngine.Source.Code.Core.Character.Sound
{
    public class BoostAudioDistortion : MonoBehaviour
    {
        private bool _enabled;
        
        [Inject] private VolumeManager _volumeManager;
        
        public void Toggle()
        {
            if (!_volumeManager.GetData().BoostDistortionEnabled) return;
            
            _enabled = !_enabled;

            if (_enabled)
            {
                RuntimeManager.StudioSystem.setParameterByName("Distort", 1);
            }
            else
            {
                RuntimeManager.StudioSystem.setParameterByName("Distort", 0);
            }
        }
    }
}