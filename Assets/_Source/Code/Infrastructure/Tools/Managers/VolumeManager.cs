using System;
using FMODUnity;
using SurgeEngine._Source.Code.Infrastructure.Tools.Services;

namespace SurgeEngine._Source.Code.Infrastructure.Tools.Managers
{
    public class VolumeManager : JsonStorageService<VolumeData>
    {
        private const string MasterVolumeKey = "Master";
        private const string MusicVolumeKey = "Music";
        private const string SFXVolumeKey = "SFX";
        private const string MenuDistortKey = "MenuDistort";

        public VolumeManager()
        {
            SetMasterVolume(Data.MasterVolume);
            SetMusicVolume(Data.MusicVolume);
            SetSFXVolume(Data.SfxVolume);
            
            ToggleMenuDistortion(false);
        }
        
        public void SetMasterVolume(float value)
        {
            Data.MasterVolume = value;
            
            RuntimeManager.StudioSystem.setParameterByName(MasterVolumeKey, Data.MasterVolume);
        }
        
        public void SetMusicVolume(float value)
        {
            Data.MusicVolume = value;
            
            RuntimeManager.StudioSystem.setParameterByName(MusicVolumeKey, Data.MusicVolume);
        }
        
        public void SetSFXVolume(float value)
        {
            Data.SfxVolume = value;
            
            RuntimeManager.StudioSystem.setParameterByName(SFXVolumeKey, Data.SfxVolume);
        }

        public void ToggleMenuDistortion(bool value)
        {
            float val = value ? 1 : 0;
            RuntimeManager.StudioSystem.setParameterByName(MenuDistortKey, val);
        }

        public void SetDistortion(bool value)
        {
            Data.BoostDistortionEnabled = value;
        }
    }

    [Serializable]
    public class VolumeData
    {
        public float MasterVolume = 1f, MusicVolume = 1f, SfxVolume = 1f;
        public bool BoostDistortionEnabled = true;
    }
}