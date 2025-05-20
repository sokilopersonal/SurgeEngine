using System;
using FMODUnity;
using SurgeEngine.Code.Infrastructure.Tools.Services;
using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers
{
    public class VolumeManager : JsonStorageService<VolumeData>
    {
        private const string MasterVolumeKey = "MasterVolume";
        private const string MusicVolumeKey = "MusicVolume";
        private const string SFXVolumeKey = "SFXVolume";

        public VolumeManager()
        {
            SetMasterVolume(Data.MasterVolume);
            SetMusicVolume(Data.MusicVolume);
            SetSFXVolume(Data.SfxVolume);
        }
        
        public void SetMasterVolume(float value)
        {
            Data.MasterVolume = value / 100f;
            
            RuntimeManager.StudioSystem.setParameterByName(MasterVolumeKey, Data.MasterVolume);
        }
        
        public void SetMusicVolume(float value)
        {
            Data.MusicVolume = value / 100f;
            
            RuntimeManager.StudioSystem.setParameterByName(MusicVolumeKey, Data.MusicVolume);
        }
        
        public void SetSFXVolume(float value)
        {
            Data.SfxVolume = value / 100f;
            
            RuntimeManager.StudioSystem.setParameterByName(SFXVolumeKey, Data.SfxVolume);
        }
    }

    [Serializable]
    public class VolumeData
    {
        public float MasterVolume = 1f, MusicVolume = 1f, SfxVolume = 1f;
    }
}