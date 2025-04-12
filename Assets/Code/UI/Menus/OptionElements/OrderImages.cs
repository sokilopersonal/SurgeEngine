using System;
using UnityEngine;
using UnityEngine.UI;

namespace SurgeEngine.Code.UI.Menus.OptionElements
{
    public class OrderImages : MonoBehaviour
    {
        [SerializeField] private Image left;
        [SerializeField] private Image right;
        [SerializeField] private Sprite arrow;
        [SerializeField] private Sprite sphere;
        
        private OptionBar _bar;

        private void Awake()
        {
            _bar = GetComponentInParent<OptionBar>();
            OnIndexChanged(_bar.Index);
        }

        private void OnEnable()
        {
            _bar.OnIndexChanged += OnIndexChanged;
        }
        
        private void OnDisable()
        {
            _bar.OnIndexChanged -= OnIndexChanged;
        }

        private void OnIndexChanged(int obj)
        {
            left.sprite = obj == 0 ? sphere : arrow;
            right.sprite = obj == _bar.States.Length - 1 ? sphere : arrow;
        }
    }
}