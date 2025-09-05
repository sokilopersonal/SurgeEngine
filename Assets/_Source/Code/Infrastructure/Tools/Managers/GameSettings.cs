using System;
using SurgeEngine._Source.Code.Infrastructure.Tools.Services;

namespace SurgeEngine._Source.Code.Infrastructure.Tools.Managers
{
    public class GameSettings : JsonStorageService<GameData>
    {
        public bool RunInBackground
        {
            get => Data.runInBackground;
            set => Data.runInBackground = value;
        }
        
        public bool IsDebug
        {
            get => Data.isDebug;
            set => Data.isDebug = value;
        }
    }

    [Serializable]
    public class GameData
    {
        public bool runInBackground = true;
        public bool isDebug;
    }
}