using SurgeEngine.Code.UI.Menus.OptionElements;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers.UI
{
    public class GameSettingsUI : OptionUI
    {
        [SerializeField] private OptionBar runInBackgroundBar;

        [Inject] private GameSettings _game;

        protected override void Awake()
        {
            base.Awake();
            
            runInBackgroundBar.OnIndexChanged += index => _game.RunInBackground = index == 1;

            var data = _game.GetData();
            runInBackgroundBar.SetIndex(data.runInBackground ? 1 : 0);
        }

        public override void Save()
        {
            _game.Save();
            
            base.Save();
        }
        
        public override void Revert()
        {
            _game.Load(data =>
            {
                runInBackgroundBar.SetIndex(data.runInBackground ? 1 : 0);
                
                Save();
            });

            IsDirty = false;
        }
    }
}