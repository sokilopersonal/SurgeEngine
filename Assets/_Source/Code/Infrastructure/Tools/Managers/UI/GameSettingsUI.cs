using SurgeEngine.Code.UI.Menus.OptionElements;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.Tools.Managers.UI
{
    public class GameSettingsUI : MonoBehaviour
    {
        [SerializeField] private OptionBar runInBackgroundBar;

        [Inject] private GameSettings _game;
        
        private void Awake()
        {
            _game.Load(data =>
            {
                runInBackgroundBar.OnIndexChanged += index => _game.RunInBackground = index == 1;
                runInBackgroundBar.SetIndex(data.runInBackground ? 1 : 0);
            });
        }

        public void Save()
        {
            _game.Save();
        }
    }
}