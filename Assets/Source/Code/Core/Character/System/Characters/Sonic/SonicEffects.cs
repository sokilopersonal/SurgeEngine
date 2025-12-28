using System.Collections;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Gameplay.Effects;
using SurgeEngine.Source.Code.Shaders;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.System.Characters.Sonic
{
    public class SonicEffects : CharacterEffects
    {
        [Header("Trail")]
        [SerializeField] private VolumeTrailRenderer trailRenderer;

        [Header("Boost")]
        [SerializeField] private Effect boostAura;
        public Effect BoostAura => boostAura;
        [SerializeField] private BoostDistortion boostDistortion;

        [Header("Spinball")] 
        [SerializeField] private Effect spinball;

        [Header("Stomping")]
        [SerializeField] private StompEffect stompEffect;

        [Header("Sliding")]
        [SerializeField] private Effect slideEffect;
        
        [Header("Sweepkick")]
        [SerializeField] private Effect sweepKickEffect;

        [Header("Light Speed Dash")]
        [SerializeField] private Effect lightSpeedDashEffect;
        
        [Header("Drift")]
        [SerializeField] private DriftEffect driftEffect;

        private FBoost _boost;

        protected override void Awake()
        {
            base.Awake();

            Character.StateMachine.GetState(out _boost);
            
            driftEffect.Rigidbody = Character.Rigidbody;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (_boost != null)
                _boost.OnActiveChanged += OnBoostActivate;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            if (_boost != null)
                _boost.OnActiveChanged -= OnBoostActivate;
        }

        protected override void OnStateAssign(FState obj)
        {
            base.OnStateAssign(obj);
            
            FState prev = Character.StateMachine.PreviousState;
            
            if (obj is FStateHoming or FStateStomp or FStateSwingJump)
                trailRenderer.Emit(0.6f);
            
            // jump visuals
            if (obj is FStateJump && prev is not FStateUpreel)
            {
                if (prev is FStatePulley)
                    StartCoroutine(PlayJumpball(0.333f));
                else if (prev is FStateJump)
                    spinball.Toggle(true);
                else
                    StartCoroutine(PlayJumpball());
            }
            else
            {
                StopCoroutine(PlayJumpball());
                spinball.Toggle(false);
            }

            lightSpeedDashEffect.Toggle(obj is FStateLightSpeedDash);

            if (obj is FStateGround or FStateStompLand && prev is FStateStomp)
                stompEffect.Land();

            stompEffect.Toggle(obj is FStateStomp);
            slideEffect.Toggle(obj is FStateSlide);

            if (obj is FStateSweepKick)
            {
                sweepKickEffect.Clear();
                sweepKickEffect.Toggle(true);
            }
            else if (prev is FStateSweepKick && obj is FStateGround or FStateIdle or FStateSit)
            {
                sweepKickEffect.Toggle(false);
            }
            else if (prev is FStateSweepKick)
            {
                sweepKickEffect.Clear();
            }
            
            driftEffect.Toggle(obj is FStateDrift);
        }
        
        private void OnBoostActivate(FSubState obj, bool value)
        {
            if (obj is not FBoost) return;
            boostAura.Toggle(value);
            var viewport = Character.Camera.GetCamera().WorldToViewportPoint(Character.transform.position);
            viewport.y *= 0.8f;
            if (value) boostDistortion.Play(viewport);
        }
        
        private IEnumerator PlayJumpball()
        {
            yield return new WaitForSeconds(Character.Config.jumpMaxShortTime);
            if (Character.StateMachine.CurrentState is FStateJump && Character.Input.AHeld)
                spinball.Toggle(true);
        }

        private IEnumerator PlayJumpball(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (Character.StateMachine.CurrentState is FStateJump && Character.Input.AHeld)
                spinball.Toggle(true);
        }
    }
}