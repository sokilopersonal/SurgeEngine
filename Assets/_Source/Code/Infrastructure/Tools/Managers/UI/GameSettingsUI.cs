using SurgeEngine.Code.UI.OptionBars;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers.UI
{
    public class GameSettingsUI : OptionUI
    {
        [SerializeField] private OptionBar runInBackgroundBar;

        [Inject] private GameSettings _gameSettings;
        
        protected override void Start()
        {
            var data = _gameSettings.GetData();
            
            runInBackgroundBar.OnChanged += b =>
            {
                _gameSettings.RunInBackground = b.Index == 1;
            };
            
            runInBackgroundBar.Set(data.runInBackground ? 1 : 0);
            
            base.Start();
        }

        public override void Save()
        {
            _gameSettings.Save();
            
            base.Save();
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