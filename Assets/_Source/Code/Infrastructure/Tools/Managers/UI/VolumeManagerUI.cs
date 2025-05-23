using SurgeEngine.Code.UI.Menus.OptionElements;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers.UI
{
    public class VolumeManagerUI : OptionUI
    {
        [SerializeField] private OptionBar masterVolumeBar;
        [SerializeField] private OptionBar musicVolumeBar;
        [SerializeField] private OptionBar sfxVolumeBar;

        [Inject] private VolumeManager _volumeManager;

        protected override void Awake()
        {
            base.Awake();
            
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

        public override void Save()
        {
            _volumeManager.Save();
            
            base.Save();
        }

        public override void Revert()
        {
            _volumeManager.Load(data =>
            {
                masterVolumeBar.SetIndex(Mathf.FloorToInt(data.MasterVolume * 100));
                musicVolumeBar.SetIndex(Mathf.FloorToInt(data.MusicVolume * 100));
                sfxVolumeBar.SetIndex(Mathf.FloorToInt(data.SfxVolume * 100));
                
                Save();
            });
        }
    }
}