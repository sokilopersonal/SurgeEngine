using SurgeEngine._Source.Code.Core.Character.States;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects
{
    public class CharacterSpawn : MonoBehaviour
    {
        [SerializeField] private StartData startData;
        public StartData StartData => startData;
    }
}