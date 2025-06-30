using SurgeEngine.Code.UI.OptionBars;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers.UI
{
    public class VolumeManagerUI : OptionUI
    {
        [SerializeField] private SliderOptionBar masterSlider;
        [SerializeField] private SliderOptionBar sfxSlider;
        [SerializeField] private SliderOptionBar musicSlider;

        [Inject] private VolumeManager _volumeManager;

        protected override void Awake()
        {
            base.Awake();

            var data = _volumeManager.GetData();

            masterSlider.OnChanged += b =>
            {
                _volumeManager.SetMasterVolume(masterSlider.Slider.value / 100f);
            };
            
            sfxSlider.OnChanged += b =>
            {
                _volumeManager.SetSFXVolume(sfxSlider.Slider.value / 100f);
            };
            
            musicSlider.OnChanged += b =>
            {
                _volumeManager.SetMusicVolume(musicSlider.Slider.value / 100f);
            };
            
            masterSlider.Slider.value = data.MasterVolume * 100;
            sfxSlider.Slider.value = data.SfxVolume * 100;
            musicSlider.Slider.value = data.MusicVolume * 100;
        }

        public override void Save()
        {
            _volumeManager.Save();
            
            base.Save();
        }

        public override void Revert()
        {
            base.Revert();

            _volumeManager.Load(data =>
            {
                masterSlider.Slider.value = data.MasterVolume;
                sfxSlider.Slider.value = data.SfxVolume;
                musicSlider.Slider.value = data.MusicVolume;

                Save();
            });
        }
    }
}