using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
{
    public class CharacterComponent : MonoBehaviour
    {
        public CharacterBase character { get; private set; }

        internal virtual void Set(CharacterBase character) => this.character = character;
    }
}