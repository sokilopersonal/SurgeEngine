using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.Sound
{
    public class HomingSound : CharacterSound
    {
        [SerializeField] private EventReference homingSound;
        
        protected override void SoundState(FState obj)
        {
            if (obj is FStateHoming)
            {
                RuntimeManager.PlayOneShot(homingSound);
            }
        }
    }
}