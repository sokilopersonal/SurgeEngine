using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class CharacterComponent : MonoBehaviour
    {
        [Inject] public CharacterBase Character { get; private set; }
    }
}