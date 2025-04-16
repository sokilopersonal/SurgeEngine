using System;
using System.IO;
using FMODUnity;
using UnityEngine;

namespace SurgeEngine.Code.Tools
{
    public class VolumeManager : IStorageService
    {
        private const string FileName = "VolumeData.json";
        
        private const string MasterVolumeKey = "MasterVolume";
        private const string MusicVolumeKey = "MusicVolume";
        private const string SFXVolumeKey = "SFXVolume";
        
        private VolumeData _data;

        public VolumeManager()
        {
            Load<VolumeData>(data => _data = data);
            
            SetMasterVolume(_data.MasterVolume);
            SetMusicVolume(_data.MusicVolume);
            SetSFXVolume(_data.SfxVolume);
        }
        
        public static void SetMasterVolume(float value)
        {
            RuntimeManager.StudioSystem.setParameterByName(MasterVolumeKey, value / 10f);
        }
        
        public static void SetMusicVolume(float value)
        {
            RuntimeManager.StudioSystem.setParameterByName(MusicVolumeKey, value / 10f);
        }
        
        public static void SetSFXVolume(float value)
        {
            RuntimeManager.StudioSystem.setParameterByName(SFXVolumeKey, value / 10f);
        }

        public void Save(Action<bool> callback = null)
        {
            if (_data != null)
            {
                var path = GetDataPath();
                RuntimeManager.StudioSystem.getParameterByName(MasterVolumeKey, out _data.MasterVolume);
                RuntimeManager.StudioSystem.getParameterByName(MusicVolumeKey, out _data.MusicVolume);
                RuntimeManager.StudioSystem.getParameterByName(SFXVolumeKey, out _data.SfxVolume);
                
                File.WriteAllText(path, JsonUtility.ToJson(_data, true));
            }
            else
            {
                _data = new VolumeData();
                Save(callback);
            }
        }

        public void Load<T>(Action<T> callback = null)
        {
            if (File.Exists(GetDataPath()))
            {
                var data = JsonUtility.FromJson<T>(File.ReadAllText(GetDataPath()));
                callback?.Invoke(data);
            }
        }

        public string GetDataPath() => Path.Combine(Application.persistentDataPath, FileName);
    }

    [Serializable]
    public class VolumeData
    {
        public float MasterVolume = 1f, MusicVolume = 1f, SfxVolume = 1f;
    }
}