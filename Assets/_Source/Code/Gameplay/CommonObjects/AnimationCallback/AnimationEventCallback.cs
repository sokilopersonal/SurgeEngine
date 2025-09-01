using System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.AnimationCallback
{
    public class AnimationEventCallback : MonoBehaviour
    {
        public event Action OnAnimationEvent;

        public void Call(string name)
        {
            OnAnimationEvent?.Invoke();
        }
    }
}