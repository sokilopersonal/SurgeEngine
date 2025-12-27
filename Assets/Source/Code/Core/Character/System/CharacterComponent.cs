using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class CharacterComponent : MonoBehaviour
    {
        public CharacterBase Character { get; private set; }

        public virtual void Set(CharacterBase character)
        {
            Character = character;
        }
    }
}