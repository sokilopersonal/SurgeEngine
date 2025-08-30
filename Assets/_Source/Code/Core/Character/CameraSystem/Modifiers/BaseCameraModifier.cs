using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Modifiers
{
    public abstract class BaseCameraModifier : MonoBehaviour
    {
        protected CharacterBase Character;
        
        [SerializeField] protected float multiplier = 2;
        
        public virtual void Set(CharacterBase character)
        {
            Character = character;
        }
    }
}