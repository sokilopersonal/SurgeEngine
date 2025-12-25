using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.Sound
{
    public class LightSpeedDashSound : CharacterSound
    {
        [SerializeField] private EventReference lightSpeedDashSound;

        protected override void SoundState(FState obj)
        {
            if (obj is FStateLightSpeedDash)
            {
                RuntimeManager.PlayOneShot(lightSpeedDashSound);
            }
        }
    }
}