using System;
using SurgeEngine.Code.Infrastructure.Tools.Services;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers
{
    [Serializable]
    public class GameData
    {
        public bool runInBackground = true;
    }

    public class GameSettings : JsonStorageService<GameData>
    {
        public bool RunInBackground
        {
            get => Data.runInBackground;
            set
            {
                Data.runInBackground = value;
                Save();
            }
        }
    }
}