using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class ParaloopSound : ActorSound
    {
        [SerializeField] private EventReference _paraloopSound;
        public void Play()
        {
            RuntimeManager.PlayOneShotAttached(_paraloopSound, gameObject);
        }
    }
}