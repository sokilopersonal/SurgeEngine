using SurgeEngine.Code.UI.Menus.OptionElements;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers.UI
{
    public class VolumeManagerUI : MonoBehaviour
    {
        [SerializeField] private OptionBar masterVolumeBar;
        [SerializeField] private OptionBar musicVolumeBar;
        [SerializeField] private OptionBar sfxVolumeBar;

        [Inject] private VolumeManager _volumeManager;

        private void Awake()
        {
            _volumeManager.Load(volumeData =>
            {
                masterVolumeBar.OnIndexChanged += index => _volumeManager.SetMasterVolume(index);
                musicVolumeBar.OnIndexChanged += index => _volumeManager.SetMusicVolume(index);
                sfxVolumeBar.OnIndexChanged += index => _volumeManager.SetSFXVolume(index);
            
                masterVolumeBar.SetIndex(Mathf.FloorToInt(volumeData.MasterVolume * 100));
                musicVolumeBar.SetIndex(Mathf.FloorToInt(volumeData.MusicVolume * 100));
                sfxVolumeBar.SetIndex(Mathf.FloorToInt(volumeData.SfxVolume * 100));
            });
        }

        public void Save()
        {
            _volumeManager.Save();
        }
    }
}