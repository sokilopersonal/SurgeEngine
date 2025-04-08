using System.Collections.Generic;
using SurgeEngine.Code.Actor.States;
using SurgeEngine.Code.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Effects
{
    public class Step : MonoBehaviour
    {
        [SerializeField] private ParticleSystem smokeParticleSystem;
        [SerializeField] private Material smokeMaterial;
        [SerializeField] private Texture2D grassSmoke;
        [SerializeField] private Texture2D ironSmoke;
        [SerializeField] private Texture2D waterSmoke;

        private Dictionary<GroundTag, Texture2D> _smokes;
        
        private static readonly int baseMap = Shader.PropertyToID("_MainTex");

        private void Awake()
        {
            _smokes = new Dictionary<GroundTag, Texture2D>()
            {
                [GroundTag.Grass] = grassSmoke,
                [GroundTag.Concrete] = ironSmoke,
                [GroundTag.Water] = waterSmoke,
            };
            
            smokeMaterial = Instantiate(smokeMaterial);
            smokeParticleSystem.GetComponent<ParticleSystemRenderer>().material = smokeMaterial;
        }

        private void Update()
        {
            ActorBase context = ActorContext.Context;

            if (context.stateMachine.IsExact<FStateGround>())
            {
                if (smokeParticleSystem.isStopped)
                {
                    smokeParticleSystem.Play();
                }
                
                smokeMaterial.SetTexture(baseMap, _smokes[context.stateMachine.GetState<FStateGround>().GetSurfaceTag()]);
            }
            else
            {
                smokeParticleSystem.Stop();
            }
        }
    }
}