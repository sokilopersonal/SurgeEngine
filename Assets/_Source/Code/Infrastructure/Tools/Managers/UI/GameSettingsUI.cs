using SurgeEngine._Source.Code.UI.OptionBars;
using UnityEngine;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.Tools.Managers.UI
{
    public class GameSettingsUI : OptionUI
    {
        [SerializeField] private OptionBar runInBackgroundBar;

        [Inject] private GameSettings _gameSettings;
        
        protected override void Setup()
        {
            var data = _gameSettings.GetData();
            
            runInBackgroundBar.OnChanged += b =>
            {
                _gameSettings.RunInBackground = b.Index == 1;
            };
            
            runInBackgroundBar.Set(data.runInBackground ? 1 : 0);
        }

        public override void Save()
        {
            base.Save();
            
            _gameSettings.Save();
        }

        public override void Revert()
        {
            base.Revert();
            
            _gameSettings.Load(data =>
            {
                runInBackgroundBar.Set(data.runInBackground ? 1 : 0);
                
                Save();
            });
        }
    }
}