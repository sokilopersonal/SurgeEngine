using SurgeEngine.Code.Infrastructure.Tools.Managers;
using UnityEngine;

namespace SurgeEngine._Source.Code.UI.MainMenu
{
    [CreateAssetMenu(fileName = "StageSlotView", menuName = "SurgeEngine/UI/StageSlotView")]
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