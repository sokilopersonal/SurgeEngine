using System.Collections.Generic;
using NaughtyAttributes;
using SurgeEngine.Code.Actor.States;
using SurgeEngine.Code.Actor.System;
using UnityEngine;
using UnityEngine.VFX;

namespace SurgeEngine.Code.ActorEffects
{
    public class Step : MonoBehaviour
    {
        [SerializeField, InfoBox("Steps spawn point")] private Transform feet;
        
        [SerializeField] private VisualEffect step;

        public void PlayEffect()
        {
            ActorBase context = ActorContext.Context;

            string surface = context.stateMachine.GetState<FStateGround>().GetSurfaceTag();
            VisualEffect stepInstance = Instantiate(step, feet.position + Vector3.up * 0.25f, Quaternion.identity);
            stepInstance.Play();
            
            Destroy(stepInstance.gameObject, stepInstance.GetFloat("Lifetime"));
        }
    }
}