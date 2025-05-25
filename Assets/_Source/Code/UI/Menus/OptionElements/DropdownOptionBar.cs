using System;
using TMPro;
using UnityEngine;

namespace SurgeEngine.Code.UI.Menus.OptionElements
{
    public class DropdownOptionBar : OptionBar
    {
        [Header("Dropdown")]
        [SerializeField] private TMP_Dropdown dropdown;
        public TMP_Dropdown Dropdown => dropdown;
        
        public event Action<int> OnDropdownBarValueChanged;

        protected override void Awake()
        {
            base.Awake();
            
            if (!dropdown)
                dropdown = GetComponentInChildren<TMP_Dropdown>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (dropdown)
                dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            
            OnBarSubmit += SelectDropdown;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            if (dropdown)
                dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
            
            OnBarSubmit -= SelectDropdown;
        }
        
        private void OnDropdownValueChanged(int value)
        {
            Index = value;
            
            OnDropdownBarValueChanged?.Invoke(value);
        }

        public override void SetIndex(int index)
        {
            base.SetIndex(index);
            
            dropdown.value = index;
            OnDropdownValueChanged(index);
        }

        public override void UpdateText(int index)
        {
        }

        private void SelectDropdown(OptionBar obj)
        {
            dropdown.Show();
        }
    }
}