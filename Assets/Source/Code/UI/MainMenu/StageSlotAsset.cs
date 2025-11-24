using SurgeEngine.Source.Code.Infrastructure.Tools.Managers;
using UnityEngine;

namespace SurgeEngine.Source.Code.UI.MainMenu
{
    [CreateAssetMenu(fileName = "StageSlotView", menuName = "Surge Engine/UI/StageSlotView")]
    public class StageSlotAsset : ScriptableObject
    {
        [SerializeField] private Sprite image;
        [SerializeField] private new string name;
        [SerializeField] private string sceneName;
        
        public Sprite Image => image;
        public string Name => name;

        public void Load()
        {
            SceneLoader.LoadGameScene(sceneName);
        }
    }
}