using System.Collections.Generic;
using NaughtyAttributes;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.ActorEffects
{
    public class Step : MonoBehaviour
    {
        [SerializeField, InfoBox("Steps spawn point")] private Transform feet;
        
        [SerializeField] private ParticleSystem waterStep;

        private Dictionary<string, ParticleSystem> _steps = new Dictionary<string, ParticleSystem>();

        private void Awake()
        {
            _steps = new Dictionary<string, ParticleSystem>()
            {
                ["Concrete"] = null,
                ["Water"] = waterStep,
            };
        }

        private void PlayStepEffect()
        {
            var context = ActorContext.Context;

            string surface = context.stateMachine.GetState<FStateGround>().GetSurfaceTag();
            Vector3 add = Vector3.zero;

            if (surface == "Water")
            {
                add = Vector3.up * 0.2f;
            }
            
            var stepPrefab = GetStep(surface);
            if (stepPrefab != null)
            {
                var step = Instantiate(stepPrefab, feet.position + add, Quaternion.identity);
                step.Play();
            
                Destroy(step.gameObject, step.main.duration);
            }
        }
        
        private ParticleSystem GetStep(string surface)
        {
            return _steps.GetValueOrDefault(surface);
        }
    }
}