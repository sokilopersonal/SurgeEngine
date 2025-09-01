using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.CameraSystem.Modifiers
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