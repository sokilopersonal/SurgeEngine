using System.Collections.Generic;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Gameplay.Effects;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.System
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
        public ParaloopEffect ParaloopEffect => paraloopEffect;

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
            Character.Kinematics.GroundTag.Changed += OnSurfaceTagChanged;
            Character.StateMachine.OnStateAssign += OnStateAssign;
        }

        protected virtual void OnDisable()
        {
            Character.Kinematics.GroundTag.Changed -= OnSurfaceTagChanged;
            Character.StateMachine.OnStateAssign -= OnStateAssign;
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
                else if (_stepMap.TryGetValue(Character.Kinematics.GroundTag.Value, out var ps))
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

        private void OnSurfaceTagChanged(GroundTag oldTag, GroundTag newTag)
        {
            if (_stepMap.TryGetValue(newTag, out var newParticle))
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
    }
}
