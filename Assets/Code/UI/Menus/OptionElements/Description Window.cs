using System;
using TMPro;
using UnityEngine;

namespace SurgeEngine.Code.UI.Menus.OptionElements
{
    public class DescriptionWindow : MonoBehaviour
    {
        [SerializeField] private OptionBar[] optionBars;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text stateDescription;

        private void Awake()
        {
            for (int i = 0; i < optionBars.Length; i++)
            {
                optionBars[i].OnBarSelected += OnBarSelected;
            }
        }

        private void OnBarSelected(OptionBar bar)
        {
            title.text = bar.OptionName;
            stateDescription.text = bar.OptionDescription;
        }
    }
}