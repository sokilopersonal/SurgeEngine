using System;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using SurgeEngine._Source.Code.Infrastructure.Tools.Services;

namespace SurgeEngine._Source.Code.Infrastructure.Tools.Managers
{
    public class UserInput : JsonStorageService<UserInputSettings>
    {
        public void SetSensitivity(float value)
        {
            Data.Sensitivity.Value = value;
        }
    }

    [Serializable]
    public class UserInputSettings
    {
        public ReactiveVar<float> Sensitivity = new(1f);
    }
}