using FMODUnity;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSoundEffects
{
    public class HomingSound : ActorSound
    {
        [SerializeField] private EventReference homingSound;

        private void OnEnable()
        {
            actor.stateMachine.OnStateAssign += OnStateAssign;
        }
        
        private void OnDisable()
        {
            actor.stateMachine.OnStateAssign -= OnStateAssign;
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is FStateHoming)
            {
                RuntimeManager.PlayOneShot(homingSound);
            }
        }
    }
}