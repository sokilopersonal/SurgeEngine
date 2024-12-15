using System.Collections;
using SurgeEngine.Code.ActorEffects;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorEffects : ActorComponent
    {
        [Header("Trail")]
        [SerializeField] private VolumeTrailRenderer trailRenderer;

        [Header("Boost")]
        [SerializeField] private Effect boostAura;
        [SerializeField] private BoostDistortion boostDistortion;

        [Header("Spinball")] 
        public Effect spinball;

        [Header("Stomping")]
        public StompEffect stompEffect;

        [Header("Sliding")]
        public Effect slideEffect;
        
        [Header("Grind")]
        public Effect grindEffect;

        private void Start()
        {
            boostAura.Toggle(false);
            spinball.Toggle(false);
            stompEffect.Toggle(false);
            slideEffect.Toggle(false);
            grindEffect.Toggle(false);
        }

        private void OnBoostActivate(FSubState obj, bool value)
        {
            if (obj is not FBoost) return;
            
            boostAura.Toggle(value);
            Vector3 viewportPosition = actor.camera.GetCamera().WorldToViewportPoint(actor.transform.position);
            viewportPosition.y *= 0.8f; // Correction because the player's pivot is in the head (wtf?)
            if (value) boostDistortion.Play(viewportPosition);
        }

        private void OnStateAssign(FState obj)
        {
            var machine = actor.stateMachine;
            var prev = actor.stateMachine.PreviousState;
            
            spinball.Toggle(machine.IsExact<FStateJump>() || machine.IsExact<FStateHoming>());
            grindEffect.Toggle(obj is FStateGrind or FStateGrindSquat);

            if (obj is FStateGround or FStateIdle && prev is FStateStomp)
            {
                stompEffect.Land();
            }
            
            stompEffect.Toggle(obj is FStateStomp);
            slideEffect.Toggle(obj is FStateSliding);
        }

        private void OnEnable()
        {
            actor.stateMachine.GetSubState<FBoost>().OnActiveChanged += OnBoostActivate;
            actor.stateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnDisable()
        {
            actor.stateMachine.GetSubState<FBoost>().OnActiveChanged -= OnBoostActivate;
            actor.stateMachine.OnStateAssign -= OnStateAssign;
        }
    }
}