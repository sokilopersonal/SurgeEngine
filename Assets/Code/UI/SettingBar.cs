using System;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI
{
    public class SettingBar : Selectable, ISubmitHandler
    {
        [SerializeField, ResizableTextArea] private string description;
        public string Description => description;

        public event Action<SettingBar> OnBarSelected;
        
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
           
            OnBarSelected?.Invoke(this);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            
        }
    }
}