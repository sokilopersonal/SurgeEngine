using System;
using UnityEngine;

namespace SurgeEngine.Code.UI
{
    public class SelectionBox : MonoBehaviour
    {
        [SerializeField] private RectTransform rect;

        private void Awake()
        {
            Vector2 delta = rect.sizeDelta;
            delta.y = 0;
            rect.sizeDelta = delta;
        }
    }
}