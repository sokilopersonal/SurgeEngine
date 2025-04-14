using System;
using SurgeEngine.Code.UI.Menus.OptionElements;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Tools
{
    public class VolumeManagerUI : MonoBehaviour
    {
        [SerializeField] private OptionBar masterVolumeBar;
        [SerializeField] private OptionBar musicVolumeBar;
        [SerializeField] private OptionBar sfxVolumeBar;

        [Inject] private VolumeManager _volumeManager;

        private void Awake()
        {
            var volumeData = _volumeManager.Load();
            
            masterVolumeBar.OnIndexChanged += index => VolumeManager.SetMasterVolume(index);
            musicVolumeBar.OnIndexChanged += index => VolumeManager.SetMusicVolume(index);
            sfxVolumeBar.OnIndexChanged += index => VolumeManager.SetSFXVolume(index);
            
            masterVolumeBar.SetIndex(Mathf.FloorToInt(volumeData.MasterVolume * 10));
            musicVolumeBar.SetIndex(Mathf.FloorToInt(volumeData.MusicVolume * 10));
            sfxVolumeBar.SetIndex(Mathf.FloorToInt(volumeData.SfxVolume * 10));
        }

        public void Save()
        {
            _volumeManager.Save();
        }
    }
}