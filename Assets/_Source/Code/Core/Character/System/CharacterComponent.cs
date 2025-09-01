using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.System
{
    public class CharacterComponent : MonoBehaviour
    {
        public CharacterBase character { get; private set; }

        internal virtual void Set(CharacterBase character) => this.character = character;
    }
}