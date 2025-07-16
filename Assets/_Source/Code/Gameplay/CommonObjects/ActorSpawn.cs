using SurgeEngine.Code.Core.Actor.States;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects
{
    public class ActorSpawn : MonoBehaviour
    {
        [SerializeField] private StartData startData;
        public StartData StartData => startData;
    }
}