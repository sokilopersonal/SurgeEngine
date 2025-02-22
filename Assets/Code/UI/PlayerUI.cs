using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SurgeEngine.Code.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("Definers")]
        [SerializeField] private UIDefinerComponent startMenu;
        [SerializeField] private UIDefinerComponent[] uiDefiners;

        [Header("Menu Change Settings")]
        [SerializeField] private Ease changeEase = Ease.OutCubic;
        [SerializeField] private float changeDuration = 0.3f;
        
        private Tween _tween;
        
        private UIDefinerComponent _currentMenu;

        private void Awake()
        {
            _currentMenu = startMenu;
        }
    }
}