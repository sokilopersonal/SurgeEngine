using System;
using System.IO;
using UnityEngine;

namespace SurgeEngine.Code.Tools
{
    public class GameSettings : IStorageService
    {
        private GameData _data;

        public GameSettings()
        {
            Load<GameData>(data => _data = data);
        }
        
        public void SetRunInBackground(bool value)
        {
            _data.runInBackground = value;
        }

        public void Save(Action<bool> callback = null)
        {
            if (_data != null)
            {
                var path = GetDataPath();
                File.WriteAllText(path, JsonUtility.ToJson(_data, true));
                callback?.Invoke(true);
            }
            else
            {
                _data = new GameData();
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

        public string GetDataPath()
        {
            return Path.Combine(Application.persistentDataPath, "GameSettings.json");
        }

        public bool GetRunInBackground() => _data.runInBackground;
    }

    [Serializable]
    public class GameData
    {
        public bool runInBackground = true;
    }
}