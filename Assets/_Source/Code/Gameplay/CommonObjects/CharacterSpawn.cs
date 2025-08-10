using SurgeEngine.Code.Core.Actor.States;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects
{
    public class CharacterSpawn : MonoBehaviour
    {
        [SerializeField] private StartData startData;
        public StartData StartData => startData;
    }
}