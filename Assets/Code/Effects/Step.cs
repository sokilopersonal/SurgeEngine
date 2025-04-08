using System.Collections.Generic;
using SurgeEngine.Code.Actor.States;
using SurgeEngine.Code.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Effects
{
    public class Step : MonoBehaviour
    {
        [SerializeField] private ParticleSystem smokeParticleSystem;
        [SerializeField] private Texture2D grassSmoke;
        [SerializeField] private Texture2D ironSmoke;
        [SerializeField] private Texture2D waterSmoke;

        private Dictionary<GroundTag, Texture2D> _smokes;
        private Renderer _renderer;
        
        private static readonly int baseMap = Shader.PropertyToID("_BaseMap");

        private void Awake()
        {
            _smokes = new Dictionary<GroundTag, Texture2D>()
            {
                [GroundTag.Grass] = grassSmoke,
                [GroundTag.Concrete] = ironSmoke,
                [GroundTag.Water] = waterSmoke,
            };

            _renderer = smokeParticleSystem.GetComponent<Renderer>();
        }

        private void Update()
        {
            ActorBase context = ActorContext.Context;

            if (context.stateMachine.IsExact<FStateGround>())
            {
                smokeParticleSystem.Play();
                _renderer.material.SetTexture(baseMap, _smokes[context.stateMachine.GetState<FStateGround>().GetSurfaceTag()]);
            }
            else
            {
                smokeParticleSystem.Stop();
            }
        }
    }
}