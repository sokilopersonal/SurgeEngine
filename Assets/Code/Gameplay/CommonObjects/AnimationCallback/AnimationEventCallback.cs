using System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.AnimationCallback
{
    public class AnimationEventCallback : MonoBehaviour
    {
        public event Action OnAnimationEvent;

        public void Call()
        {
            OnAnimationEvent?.Invoke();
        }
    }
}