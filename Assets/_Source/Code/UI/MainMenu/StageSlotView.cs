using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine._Source.Code.UI.MainMenu
{
    public class StageSlotView : MonoBehaviour, ISelectHandler
    {
        [SerializeField] private StageSlotAsset stageSlot;
        [SerializeField] private Image stageImage;
        [SerializeField] private TMP_Text stageName;

        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() => stageSlot.Load());
        }

        public void OnSelect(BaseEventData eventData)
        {
            stageImage.sprite = stageSlot.Image;
            stageName.text = stageSlot.Name;
        }
    }
}