using System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.UI
{
    public class GoalRingScreen : MonoBehaviour
    {
        private Animator _animator;

        public event Action OnFlashEnd;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void OnFlashEndEvent()
        {
            OnFlashEnd?.Invoke();
        }
    }
}