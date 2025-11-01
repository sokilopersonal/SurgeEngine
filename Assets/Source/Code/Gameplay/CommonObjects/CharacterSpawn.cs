using SurgeEngine.Source.Code.Core.Character.States;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects
{
    public class CharacterSpawn : MonoBehaviour
    {
        [SerializeField] private StartData startData;
        public StartData StartData => startData;
    }
}