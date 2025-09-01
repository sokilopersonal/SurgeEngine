using System.Collections.Generic;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using SurgeEngine._Source.Code.Gameplay.Effects;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.System
{
    public class CharacterEffects : CharacterComponent
    {
        [Header("Ground Smoke")] 
        [SerializeField] private Step step;
        private Dictionary<GroundTag, ParticleSystem> _stepMap;
        private ParticleSystem _currentStep;
        private bool _isGrounded;

        [Header("Damage")] 
        [SerializeField] private DamageEffect damageEffect;

        [Header("Other")]
        [SerializeField] private SwingEffect swingTrail;
        [SerializeField] private Effect jumpDeluxEffect;
        [SerializeField] private ParaloopEffect paraloopEffect;
        public SwingEffect SwingTrail => swingTrail;
        public Effect JumpDeluxEffect => jumpDeluxEffect;

        protected virtual void Awake()
        {
            _stepMap = new Dictionary<GroundTag, ParticleSystem>
            {
                { GroundTag.Grass, step.SmokeGrass },
                { GroundTag.Water, step.SmokeWater },
                { GroundTag.Concrete, step.SmokeIron },
            };
        }

        protected virtual void OnEnable()
        {
            character.StateMachine.GetState<FStateGround>().OnSurfaceTagChanged += OnSurfaceTagChanged;
            character.StateMachine.OnStateAssign += OnStateAssign;
        }

        protected virtual void OnDisable()
        {
            character.StateMachine.GetState<FStateGround>().OnSurfaceTagChanged -= OnSurfaceTagChanged;
            character.StateMachine.OnStateAssign -= OnStateAssign;
        }

        protected virtual void OnStateAssign(FState obj)
        {
            if (obj is FStateSwing)
            {
                swingTrail.trail.Clear();
                swingTrail.Toggle(true);
            }
            else
            {
                swingTrail.Toggle(false);
            }
            
            bool nowGrounded = obj is FStateGround or FStateCrawl or FStateBrake;
            if (_isGrounded != nowGrounded)
            {
                _isGrounded = nowGrounded;

                if (!_isGrounded)
                {
                    _currentStep?.Stop();
                }
                else if (_stepMap.TryGetValue(character.StateMachine.GetState<FStateGround>().GetSurfaceTag(), out var ps))
                {
                    _currentStep = ps;
                    _currentStep?.Play();
                }
            }

            if (obj is FStateDamage)
            {
                damageEffect.Toggle(true);
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
            paraloopEffect.startPoint = character.Kinematics.Rigidbody.position;
            paraloopEffect.sonicContext = character;
            paraloopEffect.Toggle(true);
        }
    }
}
