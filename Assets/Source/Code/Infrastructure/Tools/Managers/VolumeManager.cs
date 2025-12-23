using System;
using FMODUnity;
using SurgeEngine.Source.Code.Infrastructure.Tools.Services;

namespace SurgeEngine.Source.Code.Infrastructure.Tools.Managers
{
    public class VolumeManager : JsonStorageService<VolumeData>
    {
        private const string MasterVolumeKey = "Master";
        private const string MusicVolumeKey = "Music";
        private const string SFXVolumeKey = "SFX";
        private const string IsPausedKey = "IsPaused";
        
        private const string GameGroupBusPath = "bus:/SFX/GameGroup";
        private const string GameStatusEventPath = "event:/GameStatus";

        public VolumeManager()
        {
            SetMasterVolume(Data.masterVolume);
            SetMusicVolume(Data.musicVolume);
            SetSFXVolume(Data.sfxVolume);
            
            ToggleMenuDistortion(false);
            
            var instance = RuntimeManager.CreateInstance(GameStatusEventPath);
            instance.start();
        }
        
        public void SetMasterVolume(float value)
        {
            Data.masterVolume = value;
            
            RuntimeManager.StudioSystem.setParameterByName(MasterVolumeKey, Data.masterVolume);
        }
        
        public void SetMusicVolume(float value)
        {
            Data.musicVolume = value;
            
            RuntimeManager.StudioSystem.setParameterByName(MusicVolumeKey, Data.musicVolume);
        }
        
        public void SetSFXVolume(float value)
        {
            Data.sfxVolume = value;
            
            RuntimeManager.StudioSystem.setParameterByName(SFXVolumeKey, Data.sfxVolume);
        }

        public void ToggleMenuDistortion(bool value)
        {
            float val = value ? 1 : 0;
            RuntimeManager.StudioSystem.setParameterByName(IsPausedKey, val);
            
            RuntimeManager.StudioSystem.getBus(GameGroupBusPath, out var gameBus);
            gameBus.setPaused(value);
        }

        public void SetDistortion(bool value)
        {
            Data.boostDistortionEnabled = value;
        }
    }

    [Serializable]
    public class VolumeData
    {
        public float masterVolume = 1f, musicVolume = 1f, sfxVolume = 1f;
        public bool boostDistortionEnabled = true;
    }
}