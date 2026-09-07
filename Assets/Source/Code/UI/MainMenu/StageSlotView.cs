using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Source.Code.UI.MainMenu
{
    public class StageSlotView : MonoBehaviour, ISelectHandler
    {
        [SerializeField] private StageSlotAsset stageSlot;

        public event Action<StageSlotAsset> OnSelectEvent; 

        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() => stageSlot.Load());
        }

        public void OnSelect(BaseEventData eventData)
        {
            OnSelectEvent?.Invoke(stageSlot);
        }
    }
}