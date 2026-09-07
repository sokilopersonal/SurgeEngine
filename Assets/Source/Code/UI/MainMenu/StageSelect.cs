using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Source.Code.UI.MainMenu
{
    public class StageSelect : MonoBehaviour
    {
        [SerializeField] private Image stageImage;
        [SerializeField] private TMP_Text stageName;
        
        private List<StageSlotView> _stageSlots;

        private void Awake()
        {
            _stageSlots = new List<StageSlotView>(transform.GetComponentsInChildren<StageSlotView>());
        }

        private void OnEnable()
        {
            foreach (var slot in _stageSlots)
            {
                slot.OnSelectEvent += OnSlotSelect;
            }
        }

        private void OnDisable()
        {
            foreach (var slot in _stageSlots)
            {
                slot.OnSelectEvent -= OnSlotSelect;
            }
        }

        private void OnSlotSelect(StageSlotAsset obj)
        {
            stageImage.sprite = obj.Image;
            stageName.text = obj.Name;
        }
    }
}