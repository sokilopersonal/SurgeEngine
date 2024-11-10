using SurgeEngine.Code.ActorEffects;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorEffects : MonoBehaviour, IActorComponent
    {
        public Actor actor { get; set; }

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
            if (value) boostDistortion.Play(actor.camera.GetCamera().WorldToViewportPoint(actor.transform.position));
        }

        private void OnStateAssign(FState obj)
        {
            var prev = actor.stateMachine.PreviousState;

            spinball.gameObject.SetActive(obj is (FStateJump or FStateHoming) and not FStateGrindJump);
            
            grindEffect.Toggle(obj is FStateGrind or FStateGrindSquat);

            if (obj is FStateGround or FStateIdle && actor.stateMachine.PreviousState is FStateStomp)
            {
                stompEffect.Land();
            }
            
            stompEffect.Toggle(obj is FStateStomp);
            slideEffect.Toggle(obj is FStateSliding);
        }

        public void OnInit()
        {
            actor.stateMachine.GetSubState<FBoost>().OnActiveChanged += OnBoostActivate;
            actor.stateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnEnable()
        {
            if (actor)
            {
                actor.stateMachine.GetSubState<FBoost>().OnActiveChanged += OnBoostActivate;
                actor.stateMachine.OnStateAssign += OnStateAssign;
            }
        }

        private void OnDisable()
        {
            actor.stateMachine.GetSubState<FBoost>().OnActiveChanged -= OnBoostActivate;
            actor.stateMachine.OnStateAssign -= OnStateAssign;
        }
    }
}