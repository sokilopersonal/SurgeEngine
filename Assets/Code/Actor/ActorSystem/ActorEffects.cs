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
        [SerializeField] private BoostAura boostAura;
        [SerializeField] private BoostDistortion boostDistortion;

        private BoostDistortion _spawnedBoostDistortion;

        [Header("Spinball")] 
        public Spinball spinball;

        [Header("Stomping")]
        public Stomping stomping;

        [Header("Sliding")]
        public Sliding sliding;

        private void Start()
        {
            boostAura.enabled = false;
            spinball.enabled = false;
            stomping.enabled = false;
            sliding.enabled = false;
        }

        private void OnBoostActivate(FSubState obj, bool value)
        {
            if (obj is not FBoost) return;
            
            boostAura.enabled = value;
            if (value) boostDistortion.Play(actor.camera.GetCamera().WorldToViewportPoint(actor.transform.position));
        }

        private void OnStateAssign(FState obj)
        {
            var prev = actor.stateMachine.PreviousState;

            if (obj is (FStateJump or FStateHoming) and not FStateGrindJump)
            {
                spinball.enabled = true;
            }
            else
            {
                spinball.enabled = false;
            }

            if (obj is FStateGround or FStateIdle && actor.stateMachine.PreviousState is FStateStomp)
            {
                stomping.Land();
            }
            
            stomping.enabled = obj is FStateStomp;
            sliding.enabled = obj is FStateSliding;
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