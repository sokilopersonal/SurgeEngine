using System.Collections;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Gameplay.Effects;
using SurgeEngine.Code.Shaders;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System.Characters.Sonic
{
    public class SonicEffects : ActorEffects
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

        protected override void OnEnable()
        {
            base.OnEnable();
            
            Actor.StateMachine.GetSubState<FBoost>().OnActiveChanged += OnBoostActivate;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            Actor.StateMachine.GetSubState<FBoost>().OnActiveChanged -= OnBoostActivate;
        }

        protected override void OnStateAssign(FState obj)
        {
            base.OnStateAssign(obj);
            
            FState prev = Actor.StateMachine.PreviousState;
            
            if (obj is FStateHoming or FStateStomp or FStateSwingJump)
                trailRenderer.Emit(0.6f);
            
            // jump visuals
            if (obj is FStateJump && prev is not FStateUpreel)
            {
                if (prev is FStateJump)
                    spinball.Toggle(true);
                else
                    StartCoroutine(PlayJumpball());
            }
            else
            {
                StopCoroutine(PlayJumpball());
                spinball.Toggle(false);
            }
            
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
        }
        
        private void OnBoostActivate(FSubState obj, bool value)
        {
            if (obj is not FBoost) return;
            boostAura.Toggle(value);
            var viewport = Actor.Camera.GetCamera().WorldToViewportPoint(Actor.transform.position);
            viewport.y *= 0.8f;
            if (value) boostDistortion.Play(viewport);
        }
        
        private IEnumerator PlayJumpball()
        {
            yield return new WaitForSeconds(0.117f);
            if (Actor.StateMachine.CurrentState is FStateJump && Actor.Input.JumpHeld)
                spinball.Toggle(true);
        }
    }
}