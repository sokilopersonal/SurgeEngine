using System;

namespace SurgeEngine.Code.Tools
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