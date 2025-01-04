using System.Collections;
using SurgeEngine.Code.ActorEffects;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.Shaders;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorEffects : ActorComponent
    {
        [Header("Trail")]
        [SerializeField] private VolumeTrailRenderer trailRenderer;
        public SwingEffect swingTrail;

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

        [Header("Other")]
        public Effect jumpDeluxEffect;
        public Effect paraloopEffect;

        private void Start()
        {
            boostAura.Toggle(false);
            spinball.Toggle(false);
            stompEffect.Toggle(false);
            slideEffect.Toggle(false);
            grindEffect.Toggle(false);
        }

        public void CreateLocus(float time = 2f)
        {
            trailRenderer.Emit(time);
        }

        public void DestroyLocus(bool instant = false)
        {
            trailRenderer.Clear(instant);
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
            FStateMachine machine = actor.stateMachine;
            FState prev = actor.stateMachine.PreviousState;
            
            spinball.Toggle(machine.IsExact<FStateJump>());
            grindEffect.Toggle(obj is FStateGrind or FStateGrindSquat);

            if (obj is FStateGround or FStateIdle && prev is FStateStomp)
            {
                stompEffect.Land();
            }
            
            stompEffect.Toggle(obj is FStateStomp);
            slideEffect.Toggle(obj is FStateSlide);
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