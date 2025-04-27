using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Tools.Services
{
    public abstract class JsonStorageService<TData> where TData : new() 
    {
        protected TData Data { get; private set; }
        protected string FileName => typeof(TData).Name + ".json";

        protected JsonStorageService()
        {
            Load();
        }
        
        public void Save(Action<bool> callback = null)
        {
            Data ??= new TData();
            try
            {
                var path = GetDataPath();
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? string.Empty);
                File.WriteAllText(path, JsonConvert.SerializeObject(Data, Formatting.Indented));
                callback?.Invoke(true);
            }
            catch
            {
                callback?.Invoke(false);
            }
        }

        public void Load(Action<TData> callback = null)
        {
            var path = GetDataPath();
            if (Exists())
            {
                try
                {
                    Data = JsonConvert.DeserializeObject<TData>(File.ReadAllText(path));
                }
                catch
                {
                    Data = new TData();
                    Save();
                }
            }
            else
            {
                Data = new TData();
                Save();
            }
            callback?.Invoke(Data);
        }

        protected bool Exists()
        {
            return File.Exists(GetDataPath());
        }

        public string GetDataPath()
        {
            return Path.Combine(Application.persistentDataPath, FileName);
        }
    }
}