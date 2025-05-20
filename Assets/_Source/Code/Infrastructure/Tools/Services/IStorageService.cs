using System;

namespace SurgeEngine.Code.Infrastructure.Tools.Services
{
    public interface IStorageService
    {
        void Save(Action<bool> callback = null);
        void Load<T>(Action<T> callback = null);
        string GetDataPath();
    }
}