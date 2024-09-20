using SurgeEngine.Code.ActorEffects;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorEffects : ActorComponent
    {
        [Header("Boost")]
        [SerializeField] private BoostAura boostAura;
        [SerializeField] private BoostDistortion boostDistortion;

        [Header("Spinball")] 
        [SerializeField] private Spinball spinball;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            boostAura.enabled = false;
            spinball.enabled = false;
            
            actor.stateMachine.GetSubState<FBoost>().OnActiveChanged += OnBoostActivate;
            actor.stateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnBoostActivate(FSubState obj, bool value)
        {
            if (obj is not FBoost) return;
            
            boostAura.enabled = value;
            if (value) boostDistortion.Play(actor.camera.GetCamera().WorldToViewportPoint(actor.transform.position));
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is FStateJump)
            {
                spinball.enabled = true;
            }
            else
            {
                spinball.enabled = false;
            }
        }
    }
}