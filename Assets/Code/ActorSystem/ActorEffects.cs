using SurgeEngine.Code.ActorEffects;
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

        protected override void OnInitialized()
        {
            base.OnInitialized();
            boostAura.enabled = false;
            
            actor.stats.boost.OnActiveChanged += OnStateAssign;
        }

        private void OnStateAssign(FSubState obj, bool value)
        {
            if (obj is not FBoost) return;
            
            boostAura.enabled = value;
            if (value) boostDistortion.Play(actor.camera.GetCamera().WorldToViewportPoint(actor.transform.position));
        }
    }
}