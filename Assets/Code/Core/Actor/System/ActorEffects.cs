using System.Collections;
using System.Collections.Generic;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.SonicSpecific;
using SurgeEngine.Code.Core.Actor.States.SonicSubStates;
using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Gameplay.Effects;
using SurgeEngine.Code.Shaders;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.System
{
    public class ActorEffects : ActorComponent
    {
        [Header("Ground Smoke")] 
        [SerializeField] private Step step;
        private Dictionary<GroundTag, ParticleSystem> _stepMap;
        private ParticleSystem _currentStep;
        private bool _isGrounded;
        
        [Header("Trail")]
        [SerializeField] private VolumeTrailRenderer trailRenderer;
        public SwingEffect swingTrail;

        [Header("Boost")]
        public Effect boostAura;
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
        public ParaloopEffect paraloopEffect;
        public Effect sweepKickEffect;

        private void Awake()
        {
            boostAura.Toggle(false);
            spinball.Toggle(false);
            stompEffect.Toggle(false);
            slideEffect.Toggle(false);
            grindEffect.Toggle(false);

            _stepMap = new Dictionary<GroundTag, ParticleSystem>
            {
                { GroundTag.Grass, step.SmokeGrass },
                { GroundTag.Water, step.SmokeWater },
                { GroundTag.Concrete, step.SmokeIron },
            };
        }

        public void CreateLocus(float time = 0.6f) => trailRenderer.Emit(time);
        public void DestroyLocus(bool instant = false) => trailRenderer.Clear(instant);

        private void OnBoostActivate(FSubState obj, bool value)
        {
            if (obj is not FBoost) return;
            boostAura.Toggle(value);
            var viewport = Actor.camera.GetCamera().WorldToViewportPoint(Actor.transform.position);
            viewport.y *= 0.8f;
            if (value) boostDistortion.Play(viewport);
        }

        IEnumerator PlayJumpball()
        {
            yield return new WaitForSeconds(0.117f);
            if (Actor.stateMachine.CurrentState is FStateJump && Actor.input.JumpHeld)
                spinball.Toggle(true);
        }

        private void OnStateAssign(FState obj)
        {
            FState prev = Actor.stateMachine.PreviousState;

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

            grindEffect.Toggle(obj is FStateGrind or FStateGrindSquat);

            if ((obj is FStateGround or FStateStompLand) && prev is FStateStomp)
                stompEffect.Land();

            stompEffect.Toggle(obj is FStateStomp);
            slideEffect.Toggle(obj is FStateSlide);

            if (obj is FStateSweepKick)
            {
                sweepKickEffect.Clear();
                sweepKickEffect.Toggle(true);
            }
            else if (prev is FStateSweepKick && (obj is FStateGround or FStateIdle or FStateSit))
            {
                sweepKickEffect.Toggle(false);
            }
            else if (prev is FStateSweepKick)
            {
                sweepKickEffect.Clear();
            }

            if (obj is FStateHoming or FStateStomp or FStateSwingJump)
                CreateLocus();
            
            bool nowGrounded = obj is FStateGround or FStateCrawl or FStateBrake;
            if (_isGrounded != nowGrounded)
            {
                _isGrounded = nowGrounded;

                if (!_isGrounded)
                {
                    _currentStep?.Stop();
                }
                else if (_stepMap.TryGetValue(Actor.stateMachine.GetState<FStateGround>().GetSurfaceTag(), out var ps))
                {
                    _currentStep = ps;
                    _currentStep?.Play();
                }
            }
        }

        private void OnSurfaceTagChanged(GroundTag groundTag)
        {
            if (_stepMap.TryGetValue(groundTag, out var newParticle))
            {
                if (_currentStep != newParticle)
                {
                    _currentStep?.Stop();
                    _currentStep = newParticle;
                    if (_isGrounded)
                        _currentStep?.Play();
                }
            }
            else
            {
                _currentStep?.Stop();
                _currentStep = null;
            }
        }

        public void CreateParaloop()
        {
            paraloopEffect.startPoint = Actor.kinematics.Rigidbody.position;
            paraloopEffect.sonicContext = Actor;
            paraloopEffect.Toggle(true);
        }

        private void OnEnable()
        {
            Actor.stateMachine.GetSubState<FBoost>().OnActiveChanged += OnBoostActivate;
            Actor.stateMachine.GetState<FStateGround>().OnSurfaceTagChanged += OnSurfaceTagChanged;
            Actor.stateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnDisable()
        {
            Actor.stateMachine.GetSubState<FBoost>().OnActiveChanged -= OnBoostActivate;
            Actor.stateMachine.GetState<FStateGround>().OnSurfaceTagChanged -= OnSurfaceTagChanged;
            Actor.stateMachine.OnStateAssign -= OnStateAssign;
        }
    }
}
